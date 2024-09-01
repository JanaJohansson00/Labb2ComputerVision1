using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Labb2BildTjanster.Controllers
{
    public class ImageController : Controller
    {
        private readonly ComputerVisionService _computerVisionService;

        public ImageController(ComputerVisionService computerVisionService)
        {
            _computerVisionService = computerVisionService;
        }

        public IActionResult Index() // Metod för att visa uppladdningssidan
        {
            return View(); 
        }

        [HttpGet]
        public IActionResult Upload() // Visar uppladdningsformuläret
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile imageFile, string imageUrl, int thumbnailWidth = 100, int thumbnailHeight = 100)
        {
            if (imageFile == null && string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError(string.Empty, "Please upload an image or provide a valid image URL.");
                ViewBag.ThumbnailWidth = thumbnailWidth;
                ViewBag.ThumbnailHeight = thumbnailHeight;
                return View("Upload");
            }

            ImageAnalysis analysisResult = null;
            Stream thumbnailStream = null;

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = imageFile.OpenReadStream())
                    {
                        analysisResult = await _computerVisionService.AnalyzeImageAsync(stream);
                        thumbnailStream = await _computerVisionService.GenerateThumbnailAsync(imageFile.OpenReadStream(), thumbnailWidth, thumbnailHeight);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    {
                        ModelState.AddModelError(string.Empty, "The provided URL is not valid.");
                        return View("Upload");
                    }

                    string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    var uri = new Uri(imageUrl);
                    if (!validImageExtensions.Any(ext => uri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError(string.Empty, "The provided URL must link to an image file (e.g. .jpg, .png, .gif).");
                        ViewBag.ThumbnailWidth = thumbnailWidth;
                        ViewBag.ThumbnailHeight = thumbnailHeight;
                        return View("Upload");
                    }

                    analysisResult = await _computerVisionService.AnalyzeImageAsync(imageUrl);
                    thumbnailStream = await _computerVisionService.GenerateThumbnailAsync(imageUrl, thumbnailWidth, thumbnailHeight);
                }

                ViewBag.AnalysisResult = analysisResult;
                ViewBag.Thumbnail = ConvertToBase64(thumbnailStream);

                return View("Result");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while analyzing the image: " + ex.Message);
                ViewBag.ThumbnailWidth = thumbnailWidth;
                ViewBag.ThumbnailHeight = thumbnailHeight;
                return View("Upload");
            }
        }


        private string ConvertToBase64(Stream stream) // Konverterar en ström till en Base64-sträng
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
