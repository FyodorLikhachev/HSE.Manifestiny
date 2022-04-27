namespace HSE.Manifestiny.Dto;

public record EditRouteDto(long TripId, IEnumerable<RoutePointDto> Route);