using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSE.Manifestiny.DAL.DalModels;

[Table("users")]
public record UserDAL
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	public string Login { get; set; } = null!;
	public string Password { get; set; } = null!;
	public string? Role { get; set; }
	public string? Name { get; set; }
	public List<string>? Tags { get; set; }
	public List<string>? Categories { get; set; }
	public PriceRange? PriceRange { get; set; }
}