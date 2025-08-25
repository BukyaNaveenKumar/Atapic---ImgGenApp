using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace ImgGenApp.Services;

public class TemplateComposer
{
    private readonly IWebHostEnvironment _env;
    public TemplateComposer(IWebHostEnvironment env) { _env = env; }

    public async Task ComposeAsync(
        string? templateWebPath,
        string? logoWebPath,
        string? baseImageWebPath,
        string outputFsPath)
    {
        using var bg = await LoadOrBlankAsync(baseImageWebPath)
                  ?? await LoadOrBlankAsync(templateWebPath)
                  ?? new Image<Rgba32>(1024, 1024);

        if (!string.IsNullOrWhiteSpace(logoWebPath))
        {
            using var logo = await LoadAsync(logoWebPath);
            var targetWidth = bg.Width / 4;
            logo.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(targetWidth, 0),
                Mode = ResizeMode.Max
            }));
            var pos = new Point(bg.Width - logo.Width - 24, bg.Height - logo.Height - 24);
            bg.Mutate(x => x.DrawImage(logo, pos, 1f));
        }

        await bg.SaveAsync(outputFsPath, new PngEncoder());
    }

    private async Task<Image<Rgba32>> LoadAsync(string webPath)
    {
        var fsPath = WebToFs(webPath);
        return await Image.LoadAsync<Rgba32>(fsPath);
    }

    private async Task<Image<Rgba32>?> LoadOrBlankAsync(string? webPath)
    {
        if (string.IsNullOrWhiteSpace(webPath)) return null;
        var fsPath = WebToFs(webPath);
        if (!File.Exists(fsPath)) return null;
        return await Image.LoadAsync<Rgba32>(fsPath);
    }

    private string WebToFs(string webPath)
    {
        var withoutSlash = webPath.TrimStart('/');
        return Path.Combine(_env.WebRootPath, withoutSlash.Replace('/', Path.DirectorySeparatorChar));
    }
}
