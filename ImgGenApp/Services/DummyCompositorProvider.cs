using System.Threading.Tasks;
using ImgGenApp.Models;

namespace ImgGenApp.Services;

public class DummyCompositorProvider : IImageGenProvider
{
    private readonly FileStorageService _files;
    private readonly TemplateComposer _composer;

    public DummyCompositorProvider(FileStorageService files, TemplateComposer composer)
    {
        _files = files; _composer = composer;
    }

    public async Task<GenerationResult> GenerateAsync(
        string prompt, string? templatePath, string? logoPath, string? baseImagePath)
    {
        var outputFs = _files.NewOutputPath(out var publicUrl);
        await _composer.ComposeAsync(templatePath, logoPath, baseImagePath, outputFs);

        return new GenerationResult
        {
            OutputUrl = publicUrl,
            Provider = "Local-Compose",
            ProviderInfo = "ImageSharp overlay"
        };
    }
}
