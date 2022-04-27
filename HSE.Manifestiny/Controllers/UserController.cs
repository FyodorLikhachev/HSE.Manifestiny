using HSE.Manifestiny.DAL;
using HSE.Manifestiny.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSE.Manifestiny.Controllers;

[ApiController, Authorize]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private readonly ILogger<UserController> _logger;
	private readonly ApplicationContext _context;

	public UserController(ILogger<UserController> logger,
		ApplicationContext context)
	{
		_logger = logger;
		_context = context;
	}

	[HttpGet]
	public async Task<UserDto> GetUser()
	{
		var currentUserId = int.Parse(User.Identity!.Name!);
		var user = await _context.Users.FirstAsync(x => x.Id == currentUserId);
		var result = new UserDto(user.Name, user.Tags, user.Categories, user.PriceRange);

		return result;
	}

	[HttpPost]
	public async Task<IActionResult> CreateUser(UserDto user)
	{
		await UpdateUserData(user);
		return Ok();
	}

	[HttpPut]
	public async Task<IActionResult> EditUser(UserDto user)
	{
		await UpdateUserData(user);
		return Ok();
	}

	private async Task UpdateUserData(UserDto user)
	{
		var currentUserId = int.Parse(User.Identity!.Name!);
		var currenUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);

		currenUser!.Name = user.Name;
		currenUser.Tags = user.Tags;
		currenUser.Categories = user.Categories;
		currenUser.PriceRange = user.PriceRange;

		_context.Users.Update(currenUser);
		await _context.SaveChangesAsync();
	}
}