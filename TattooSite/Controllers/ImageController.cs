using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TattooSite.Data;
using TattooSite.Models;

namespace TattooSite.Controllers
{
    [Authorize] // Ensure only authenticated users can upload images
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _dbContext;

        public ImageController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UploadBodyImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming you're using ASP.NET Core Identity

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Convert the image to a byte array
            byte[] imageData;
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }

            // Create a new UserImage object
            var userImage = new UserImage
            {
                UserId = userId,
                ImageData = imageData,
                ImageType = image.ContentType,
                ImageName = image.FileName
            };

            // Save the image to the database
            _dbContext.UserImages.Add(userImage);
            await _dbContext.SaveChangesAsync();

            // Return the URL of the uploaded image
            string imageUrl = Url.Action("GetImage", "Image", new { id = userImage.Id });
            return Json(new { imageUrl = imageUrl });
        }

        [HttpGet]
        public IActionResult GetImage(int id)
        {
            var userImage = _dbContext.UserImages.Find(id);

            if (userImage == null)
            {
                return NotFound();
            }

            return File(userImage.ImageData, userImage.ImageType);
        }
    }
}