using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HSE.Manifestiny.Configuration;
using HSE.Manifestiny.DAL;
using HSE.Manifestiny.DAL.DalModels;
using HSE.Manifestiny.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HSE.Manifestiny.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : Controller
{
	private readonly ApplicationContext _context;

	public AccountController(ApplicationContext context)
	{
		_context = context;
	}

	[AllowAnonymous]
	[HttpPost("/signup")]
	public async Task<ActionResult<TokenDto>> SignUp(AccountDto account)
	{
		var userEntity = new UserDAL { Login = account.Login, Password = account.Password, Role = "user" };

		var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Login == account.Login);
		if (existingUser != null) return BadRequest(new { errorText = "Login is already taken" });
		
		await _context.Users.AddAsync(userEntity);
		await _context.SaveChangesAsync();
		
		var identity = GetIdentity(account);
		if (identity == null)
		{
			return BadRequest(new { errorText = "Invalid username or password." });
		}

		var token = GetToken(identity);

		return Json(token);
	}

	[AllowAnonymous]
	[HttpPost("/token")]
	public ActionResult<TokenDto> Token(AccountDto account)
	{
		var identity = GetIdentity(account);
		if (identity == null)
		{
			return BadRequest(new { errorText = "Invalid username or password." });
		}

		var token = GetToken(identity);

		return Json(token);
	}

	private TokenDto GetToken(ClaimsIdentity identity)
	{
		var now = DateTime.UtcNow;
		var jwt = new JwtSecurityToken(
			issuer: AuthOptions.ISSUER,
			audience: AuthOptions.AUDIENCE,
			notBefore: now,
			claims: identity.Claims,
			expires: now.AddHours(3),
			signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
		var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

		return new TokenDto(encodedJwt);
	}

	private ClaimsIdentity? GetIdentity(AccountDto account)
	{
		var person = _context.Users.FirstOrDefault(
			x => x.Login == account.Login && x.Password == account.Password);
		if (person == null) return null;
		var claims = new List<Claim>
		{
			new(ClaimsIdentity.DefaultNameClaimType, person.Id.ToString()),
			new(ClaimsIdentity.DefaultRoleClaimType, person.Role)
		};
		var claimsIdentity =
			new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
				ClaimsIdentity.DefaultRoleClaimType);

		return claimsIdentity;
	}
}