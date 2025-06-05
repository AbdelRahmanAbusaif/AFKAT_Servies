namespace AK_Services.DTOS;

public class PlayerSavesDTO
{
    public int PlayerId { get; init; }
    public int GameId { get; init; }
    public string FileSaveURL { get; init; }
    public IFormFile FileSave { get; init; }
}