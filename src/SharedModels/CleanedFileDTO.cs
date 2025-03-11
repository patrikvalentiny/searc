namespace SharedModels;

public class CleanedFileDTO : PropagatedMessage
{
    public string Filename { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
