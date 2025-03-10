namespace IndexerService.Domain.Models;

public class Occurences
{
    public int WordId { get; set; }
    public int FileId { get; set; }
    public int Count { get; set; }
}