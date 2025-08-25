using Microsoft.AspNetCore.Http;

namespace ImgGenApp.Models;

public class GenerationRequest
{
    public string Prompt { get; set; } = "";
    public string Template { get; set; } = "Plain"; // Plain, Office, Gradient
    public IFormFile? LogoFile { get; set; }
    public IFormFile? ImageFile { get; set; } // optional base image
}
