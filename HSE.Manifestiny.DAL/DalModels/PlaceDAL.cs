using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HSE.Manifestiny.DAL.DalModels;

[Table("places")]
public class PlaceDAL
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long PlaceId { get; set; }
	[ForeignKey("City")]
	public long CityId { get; set; }
	public string Name { get; set; } = null!;
	public string Description { get; set; } = null!;
	public string? Contacts { get; set; }
	public string? Url { get; set; }
	public int VisitDuration { get; set; }
	public List<string> Categories { get; set; } = null!;
	public List<string> Tags { get; set; } = null!;

	[JsonConverter(typeof(StringEnumConverter))]
	public PriceRange PriceRange { get; set; }
	public double Lat { get; set; }
	public double Lon { get; set; }

	public CityDAL City { get; set; } = null!;
}

public enum PriceRange
{
	Cheap,
	Average,
	Expensive
}