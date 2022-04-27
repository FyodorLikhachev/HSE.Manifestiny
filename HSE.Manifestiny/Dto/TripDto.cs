using HSE.Manifestiny.DAL.DalModels;

namespace HSE.Manifestiny.Dto;

public record TripDto(long TripId, long CityId, DateTime Since, DateTime To, TravelType TravelType);

public record CreateTripDto(long CityId, DateTime Since, DateTime To, TravelType TravelType);
