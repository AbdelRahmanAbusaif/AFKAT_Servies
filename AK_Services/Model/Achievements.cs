using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies
{
	[Microsoft.EntityFrameworkCore.Keyless]
    public class Achievements : BaseModel
	{
		[Required]
		[Supabase.Postgrest.Attributes.PrimaryKey("Id", false)]
		public int Id { get; set; }
		[Required]
		public int GameId { get; set; }
		[Required]
		public string AchivementName { get; set; }
		[Required]
		public string AchivementDescription { get; set; }
		[Required]
		public string AchivementIconURL { get; set; }
	}
}
