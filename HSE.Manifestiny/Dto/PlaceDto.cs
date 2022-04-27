using HSE.Manifestiny.DAL.DalModels;

namespace HSE.Manifestiny.Dto;

public record PlaceDto(long PlaceId, long CityId, string Name, string Description, string? Contacts,
	string? Url, int VisitDuration, List<string> Categories, List<string> Tags, PriceRange PriceRange,
	double Lat, double Lon);