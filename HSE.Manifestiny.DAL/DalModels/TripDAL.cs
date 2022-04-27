using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HSE.Manifestiny.DAL.DalModels;

[Table("trips")]
public class TripDAL{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long TripId { get; set; }
	[ForeignKey("User")]
	public long UserId { get; set; }
	[ForeignKey("City")]
	public long CityId { get; set; }
	[Column(TypeName = "jsonb")]
	public List<RoutePointDAL>? Route { get; set; }
	public DateTime Since { get; set; }
	public DateTime To { get; set; }
	[JsonConverter(typeof(StringEnumConverter))]
	public TravelType TravelType { get; set; }
	public UserDAL User { get; set; } = null!;
	public CityDAL City { get; set; } = null!;
}

public enum TravelType
{
	ByCar,
	OnFoot,
	Bicycle
}

public class TripConfiguration : IEntityTypeConfiguration<TripDAL>
{
	private JsonSerializerSettings JsonNullSetting = new() { NullValueHandling = NullValueHandling.Ignore };
	public void Configure(EntityTypeBuilder<TripDAL> builder)
	{
		builder.Property(e => e.Route).HasConversion(
			v => JsonConvert.SerializeObject(v, JsonNullSetting),
			v => JsonConvert.DeserializeObject<List<RoutePointDAL>>(v, JsonNullSetting));
	}
}
public class RoutePointDAL
{
	public long PlaceId { get; set; }
	[JsonConverter(typeof(StringEnumConverter))]
	public Status Status { get; set; }
	public DateTime VisitStart { get; set; }
	public DateTime VisitEnd { get; set; }
}

public enum Status { Upcoming, InProcess, Visited }
