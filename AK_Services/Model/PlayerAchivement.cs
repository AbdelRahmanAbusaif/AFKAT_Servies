using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies
{
	[Microsoft.EntityFrameworkCore.Keyless]
    public class PlayerAchivement : BaseModel
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public int GameId { get; set; }
		[Required]
		public int PlayerId { get; set; }
		[Required]
		public DateTime UnloackedAt { get; set; }
		[Required]
		public string AchivementIconURL { get; set; }
	}
}
