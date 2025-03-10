namespace IndexerService.Domain.Models;

public class Occurrences
{
    public int WordId { get; set; }
    public int FileId { get; set; }
    public int Count { get; set; }
}