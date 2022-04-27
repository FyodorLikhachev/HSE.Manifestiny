using HSE.Manifestiny.DAL;
using HSE.Manifestiny.DAL.DalModels;
using HSE.Manifestiny.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSE.Manifestiny.Controllers;

[ApiController, Authorize]
[Route("[controller]")]
public class TripController : ControllerBase
{
	private readonly ILogger<TripController> _logger;
	private readonly ApplicationContext _context;

	public TripController(ILogger<TripController> logger,
		ApplicationContext context)
	{
		_logger = logger;
		_context = context;
	}

	[HttpGet("getAll")]
	public async Task<IEnumerable<TripDto>> GetAllTrips()
	{
		var currentUserId = int.Parse(User.Identity!.Name!);
		var trips = await _context.Trips.Where(x => x.UserId == currentUserId).ToListAsync();

		return trips.Select(x => new TripDto(x.TripId, x.CityId, x.Since, x.To, x.TravelType));
	}

	[HttpPost]
	public async Task<IActionResult> CreateTrip(CreateTripDto trip)
	{
		var currentUserId = int.Parse(User.Identity!.Name!);
		var currentUser = await _context.Users.FirstAsync(x => x.Id == currentUserId);

		var userTrips = await _context.Trips.Where(x => x.UserId == currentUserId).ToListAsync();
		if (userTrips.Any(x => x.Since < trip.To && trip.Since < x.To))
			return BadRequest(new { errorText = "You've already scheduled trips for that period of time" });

		var allPlaces = await _context.Places
			.Where(x => x.PriceRange == currentUser.PriceRange)
			.ToListAsync();

		var interestingPlaces = allPlaces
			.Where(x => x.Categories.Intersect(currentUser.Categories!).Any() ||
						x.Tags.Intersect(currentUser.Tags!).Any()).ToList();
		var interestingPlacesId = interestingPlaces.Select(x => x.PlaceId).ToHashSet();

		var dayLimit = 8 * 60;
		var place = interestingPlaces.FirstOrDefault();
		if (place is null) return BadRequest(new { errorText = "No matching result for your filters :(" });
		var filteredTravelTime = await _context.TravelTime.Where(x => interestingPlacesId.Contains(x.PlaceBId)).ToListAsync();

		var startHours = TimeSpan.FromHours(12);
		var lastRoutePointOffset = startHours + TimeSpan.FromMinutes(place.VisitDuration);
		var route = new List<RoutePointDAL> 
		{
			new()
			{
				PlaceId = place.PlaceId,
				Status = Status.Upcoming,
				VisitStart = trip.Since + startHours,
				VisitEnd = trip.Since + lastRoutePointOffset
			}
		};

		var sum = place.VisitDuration;
		while (sum < dayLimit)
		{
			var nextPlaceTravelTime = filteredTravelTime.Where(x => x.PlaceAId == place.PlaceId)
				.OrderBy(x => x.TravelTime).FirstOrDefault();
			filteredTravelTime.RemoveAll(x => x.PlaceAId == place.PlaceId || x.PlaceBId == place.PlaceId);

			if (nextPlaceTravelTime is null) break;

			place = interestingPlaces.First(x => x.PlaceId == nextPlaceTravelTime.PlaceBId);
			
			if (sum + nextPlaceTravelTime.TravelTime + place.VisitDuration > dayLimit) break;
			
			sum += nextPlaceTravelTime.TravelTime + place.VisitDuration;
			var travelTime = TimeSpan.FromMinutes(nextPlaceTravelTime.TravelTime);
			var visitTime = TimeSpan.FromMinutes(place.VisitDuration);
			var nextRoutePoint = new RoutePointDAL
			{
				PlaceId = place.PlaceId,
				Status = Status.Upcoming,
				VisitStart = trip.Since + lastRoutePointOffset + travelTime,
				VisitEnd = trip.Since + lastRoutePointOffset + travelTime + visitTime
			};
			
			route.Add(nextRoutePoint);
		}

		var newTrip = new TripDAL
		{
			UserId = currentUserId,
			CityId = trip.CityId,
			Since = trip.Since,
			To = trip.To,
			TravelType = trip.TravelType,
			Route = route
		};

		await _context.Trips.AddAsync(newTrip);
		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPut]
	public async Task<IActionResult> EditTrip(TripDto trip)
	{
		var dbTrip = await _context.Trips.FirstOrDefaultAsync(x => x.TripId == trip.TripId);
		if (dbTrip is null) return BadRequest(new { errorText = "Passed unknown trip" });
		if (dbTrip.Route!.Any(x => x.Status != Status.Upcoming)) return BadRequest(new { errorText = "You can't change trip in process" });

		var currentUserId = int.Parse(User.Identity!.Name!);
		var currentUser = await _context.Users.FirstAsync(x => x.Id == currentUserId);

		var userTrips = await _context.Trips.Where(x => x.UserId == currentUserId).ToListAsync();
		if (userTrips.Any(x => x.Since < trip.To && trip.Since < x.To))
			return BadRequest(new { errorText = "You've already scheduled trips for that period of time" });

		var allPlaces = await _context.Places
			.Where(x => x.PriceRange == currentUser.PriceRange)
			.ToListAsync();

		var interestingPlaces = allPlaces
			.Where(x => x.Categories.Intersect(currentUser.Categories!).Any() ||
						x.Tags.Intersect(currentUser.Tags!).Any()).ToList();
		var interestingPlacesId = interestingPlaces.Select(x => x.PlaceId).ToHashSet();

		var dayLimit = 8 * 60;
		var place = interestingPlaces.FirstOrDefault();
		if (place is null) return BadRequest(new { errorText = "No matching result for your filters :(" });
		var filteredTravelTime = await _context.TravelTime.Where(x => interestingPlacesId.Contains(x.PlaceBId)).ToListAsync();

		var startHours = TimeSpan.FromHours(12);
		var lastRoutePointOffset = startHours + TimeSpan.FromMinutes(place.VisitDuration);
		var route = new List<RoutePointDAL> 
		{
			new()
			{
				PlaceId = place.PlaceId,
				Status = Status.Upcoming,
				VisitStart = trip.Since + startHours,
				VisitEnd = trip.Since + lastRoutePointOffset
			}
		};

		var sum = place.VisitDuration;
		while (sum < dayLimit)
		{
			var nextPlaceTravelTime = filteredTravelTime.Where(x => x.PlaceAId == place.PlaceId)
				.OrderBy(x => x.TravelTime).FirstOrDefault();
			filteredTravelTime.RemoveAll(x => x.PlaceAId == place.PlaceId || x.PlaceBId == place.PlaceId);

			if (nextPlaceTravelTime is null) break;

			place = interestingPlaces.First(x => x.PlaceId == nextPlaceTravelTime.PlaceBId);
			
			if (sum + nextPlaceTravelTime.TravelTime + place.VisitDuration > dayLimit) break;
			
			sum += nextPlaceTravelTime.TravelTime + place.VisitDuration;
			var travelTime = TimeSpan.FromMinutes(nextPlaceTravelTime.TravelTime);
			var visitTime = TimeSpan.FromMinutes(place.VisitDuration);
			var nextRoutePoint = new RoutePointDAL
			{
				PlaceId = place.PlaceId,
				Status = Status.Upcoming,
				VisitStart = trip.Since + lastRoutePointOffset + travelTime,
				VisitEnd = trip.Since + lastRoutePointOffset + travelTime + visitTime
			};
			
			route.Add(nextRoutePoint);
		}

		var newTrip = new TripDAL
		{
			UserId = currentUserId,
			CityId = trip.CityId,
			Since = trip.Since,
			To = trip.To,
			TravelType = trip.TravelType,
			Route = route
		};

		_context.Trips.Update(newTrip);
		await _context.SaveChangesAsync();

		return Ok();
	}
}