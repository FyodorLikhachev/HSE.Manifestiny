using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSE.Manifestiny.DAL.DalModels;

[Table("travelTimes")]
public class TravelTimeDAL
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	[ForeignKey("Place")]
	public long PlaceAId { get; set; }
	[ForeignKey("Place")]
	public long PlaceBId { get; set; }
	public int TravelTime { get; set; }
}