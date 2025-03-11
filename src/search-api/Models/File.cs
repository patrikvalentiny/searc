namespace Searc.SearchApi.Models;

public class File
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    
}
