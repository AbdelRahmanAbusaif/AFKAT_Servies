using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies
{
	[Keyless]
	public class LeaderboardEntries : BaseModel
	{
		[Required]
		[Supabase.Postgrest.Attributes.PrimaryKey()]
		public int Id { get; set; }
		[Required]
		public int PlayerId { get; set; }
		[Required]
		public int LeaderboardId { get; set; }
		[Required]
		public string PlayerName { get; set; }
		[Required]
		public int Score { get; set; }
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public DateTime UpdatedAt { get; set; }
	}
}
