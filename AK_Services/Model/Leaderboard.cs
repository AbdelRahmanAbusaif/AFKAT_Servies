using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies
{
	[Keyless]
    public class Leaderboards : BaseModel
	{
		[Required]
		[Supabase.Postgrest.Attributes.PrimaryKey("Id", false)]
		public int Id { get; set; }
		[Required]
		public int GameId { get; set; }
		[Required]
		public string LeaderboardName { get; set; }
	}
}
