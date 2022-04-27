using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSE.Manifestiny.DAL.DalModels;

[Table("cities")]
public class CityDAL{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long CityId { get; set; }
	public string Name { get; set; } = null!;
	public string Description { get; set; } = null!;
	public string TimeZone { get; set; } = null!;
};