using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env) => _env = env;

        public async Task<string?> SaveImageAsync(IFormFile file)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) return null;

            // Use ContentRootPath/wwwroot if WebRootPath is null
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "posts");
            Directory.CreateDirectory(uploadsFolder); // creates wwwroot/uploads/posts if not exists

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/posts/{fileName}";
        }

        public void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var filePath = Path.Combine(webRoot, imageUrl.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }
}