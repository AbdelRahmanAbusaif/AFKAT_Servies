using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies
{
    public class Achivements : BaseModel
    {
		[Required]
		[PrimaryKey]
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
