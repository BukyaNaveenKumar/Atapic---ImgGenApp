using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImgGenApp.Models;
using ImgGenApp.Services;
using System;

namespace ImgGenApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IImageGenProvider _gen;
        private readonly FileStorageService _files;

        public HomeController(IImageGenProvider gen, FileStorageService files)
        {
            _gen = gen; _files = files;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [RequestSizeLimit(20_000_000)] // 20 MB
        public async Task<IActionResult> Generate(GenerationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Prompt) && req.ImageFile is null && req.LogoFile is null)
            {
                ModelState.AddModelError(string.Empty, "Provide a prompt or an image.");
                return View("Index", req);
            }

            string? logo = null, image = null, template = null;

            try
            {
                // Save files and get their paths
                if (req.LogoFile != null)
                    logo = await _files.SaveAsync(req.LogoFile, "logo");
                if (req.ImageFile != null)
                    image = await _files.SaveAsync(req.ImageFile, "base");
                template = _files.GetTemplatePath(req.Template);

                // Generate image
                var result = await _gen.GenerateAsync(req.Prompt, template, logo, image);

                ViewBag.OutputUrl = result.OutputUrl;
                return View("Result");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                return View("Index", req);
            }
        }
    }
}