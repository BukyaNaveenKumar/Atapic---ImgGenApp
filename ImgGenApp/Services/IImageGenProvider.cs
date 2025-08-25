using System.Threading.Tasks;
using ImgGenApp.Models;

namespace ImgGenApp.Services;

public interface IImageGenProvider
{
    Task<GenerationResult> GenerateAsync(
        string prompt,
        string? templatePath,
        string? logoPath,
        string? baseImagePath);
}
