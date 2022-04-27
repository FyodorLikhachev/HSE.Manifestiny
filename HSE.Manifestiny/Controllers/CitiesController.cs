using HSE.Manifestiny.DAL;
using HSE.Manifestiny.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSE.Manifestiny.Controllers;

[ApiController, Authorize]
[Route("[controller]")]
public class CitiesController : ControllerBase
{
	private readonly ILogger<CitiesController> _logger;
	private readonly ApplicationContext _context;

	public CitiesController(ILogger<CitiesController> logger,
		ApplicationContext context)
	{
		_logger = logger;
		_context = context;
	}

	[HttpGet]
	public async Task<IEnumerable<CityDto>> GetCities()
	{
		var cities = await _context.Cities.ToListAsync();

		return cities.Select(x => new CityDto(x.CityId, x.Name, x.Description, x.TimeZone));
	}
}