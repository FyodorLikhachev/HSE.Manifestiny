using HSE.Manifestiny.DAL.DalModels;

namespace HSE.Manifestiny.Dto;

public record UserDto(string? Name, List<string>? Tags, List<string>? Categories, PriceRange? PriceRange);
