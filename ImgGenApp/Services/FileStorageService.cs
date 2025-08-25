using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Text;

namespace ImgGenApp.Services;

public class FileStorageService
{
    private readonly string _uploads;
    private readonly string _outputs;
    private readonly string _templates;

    public FileStorageService(IWebHostEnvironment env)
    {
        _uploads = Path.Combine(env.WebRootPath, "uploads");
        _outputs = Path.Combine(env.WebRootPath, "outputs");
        _templates = Path.Combine(env.WebRootPath, "templates");

        Directory.CreateDirectory(_uploads);
        Directory.CreateDirectory(_outputs);
        Directory.CreateDirectory(_templates);
    }

    public async Task<string?> SaveUploadAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not (".png" or ".jpg" or ".jpeg"))
            throw new InvalidOperationException("Unsupported file type");

        var name = $"{RandomName()}{ext}";
        var path = Path.Combine(_uploads, name);
        using var fs = File.Create(path);
        await file.CopyToAsync(fs);
        return $"/uploads/{name}";
    }

    public string? GetTemplatePath(string template)
    {
        var file = template switch
        {
            "Office" => "office.png",
            "Gradient" => "gradient.png",
            _ => "plain.png"
        };
        var full = Path.Combine(_templates, file);
        return File.Exists(full) ? $"/templates/{file}" : null;
    }

    public string NewOutputPath(out string publicUrl)
    {
        var name = $"{RandomName()}.png";
        var path = Path.Combine(_outputs, name);
        publicUrl = $"/outputs/{name}";
        return path;
    }

    private static string RandomName()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public async Task<string> SaveAsync(IFormFile file, string prefix)
    {
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{prefix}_{Guid.NewGuid():N}{ext}";
        var dir = Path.Combine("wwwroot", "templates");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the relative URL for use in views
        return $"/templates/{fileName}";
    }
}
