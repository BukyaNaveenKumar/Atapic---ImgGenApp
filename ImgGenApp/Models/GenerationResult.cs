namespace ImgGenApp.Models;

public class GenerationResult
{
    public string OutputUrl { get; set; } = "";
    public string? Provider { get; set; }
    public string? ProviderInfo { get; set; }
}
