using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;

namespace AFKAT_Servies;

public class PlayerSaves : BaseModel
{
    [Supabase.Postgrest.Attributes.PrimaryKey("Id", false)]
    [Required]
    public int Id { get; set; }
    [Required]
    public int PlayerId { get; set; }
    [Required]
    public int GameId { get; set; }
    [Required]
    public string FileName { get; set; }
    [Required]
    public string FileSaveURL { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
}