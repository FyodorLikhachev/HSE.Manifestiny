using HSE.Manifestiny.DAL.DalModels;

namespace HSE.Manifestiny.Dto;

public record RoutePointDto(long PlaceId, Status Status, DateTime VisitStart, DateTime VisitEnd);