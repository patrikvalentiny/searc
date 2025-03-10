namespace SharedModels;

public class IndexedFileDTO
{
    public string Filename { get; set; } = string.Empty;
    public Dictionary<string, int> WordCount { get; set; } = [];
    public byte[] Content { get; set; } = [];
}
