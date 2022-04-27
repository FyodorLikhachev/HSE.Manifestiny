using HSE.Manifestiny.DAL;
using HSE.Manifestiny.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSE.Manifestiny.Controllers;

[ApiController, Authorize]
[Route("[controller]")]
public class PlacesController : ControllerBase
{
	private readonly ILogger<PlacesController> _logger;
	private readonly ApplicationContext _context;

	public PlacesController(ILogger<PlacesController> logger,
		ApplicationContext context)
	{
		_logger = logger;
		_context = context;
	}

	[HttpGet]
	public async Task<IEnumerable<PlaceDto>> GetPlaces(long cityId, CancellationToken token)
	{
		var places =
			await _context.Places.Where(x => x.CityId == cityId).ToListAsync(token);

		var result = places.Select(x =>
			new PlaceDto(x.PlaceId, x.CityId, x.Name, x.Description, x.Contacts, x.Url, x.VisitDuration,
				x.Categories, x.Tags, x.PriceRange, x.Lat, x.Lon));

		return result;
	}
}