namespace Searc.SearchApi.Models;

public class FileContent {
    public string Filename { get; set; } = string.Empty;
    public byte[]? Content { get; set; } = Array.Empty<byte>();
}