using HSE.Manifestiny.DAL;
using HSE.Manifestiny.DAL.DalModels;
using HSE.Manifestiny.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSE.Manifestiny.Controllers;

[ApiController, Authorize]
[Route("[controller]")]
public class RouteController : ControllerBase
{
	private readonly ILogger<RouteController> _logger;
	private readonly ApplicationContext _context;

	public RouteController(ILogger<RouteController> logger,
		ApplicationContext context)
	{
		_logger = logger;
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<RoutePointDto>>> GetRoute([FromQuery]long tripId)
	{
		var trip = await GetTrip(tripId);
		if (trip is null) return BadRequest(new { errorText = $"Unknown trip #{tripId}" });

		var routePoints = trip.Route!.Select(
			x => new RoutePointDto(x.PlaceId, x.Status, x.VisitStart, x.VisitEnd));

		return Ok(routePoints);
	}

	// [HttpPut]
	// public async Task<IActionResult> EditRoute(EditRouteDto request)
	// {
	// 	var trip = await GetTrip(request.TripId);
	// 	if (trip is null) return BadRequest(new { errorText = $"Unknown trip #{request.TripId}" });
	//
	// 	return Ok();
	// }

	private async Task<TripDAL?> GetTrip(long tripId)
	{
		var currentUserId = int.Parse(User.Identity!.Name!);

		return await _context.Trips
			.Where(x => x.UserId == currentUserId && x.TripId == tripId)
			.FirstOrDefaultAsync();
	} 
}