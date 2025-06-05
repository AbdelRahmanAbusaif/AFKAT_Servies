using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies
{
	[Microsoft.EntityFrameworkCore.Keyless]
    public class PlayerAchievement : BaseModel
	{
		[Required]
		[Supabase.Postgrest.Attributes.PrimaryKey("Id", false)]
		public int Id { get; set; }
		[Required]
		public int AchievementId { get; set; }
		[Required]
		public int GameId { get; set; }
		[Required]
		public int PlayerId { get; set; }
		[Required]
		public DateTime UnlockedAt { get; set; }
		[Required]
		public string AchievementIconURL { get; set; }
	}
}
