using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TourManagementApi.Controllers;
namespace TourManagementApi.Helper
{
    public class FileHelper
    {
        public async Task<string> SaveResizedImageAsync(IFormFile file, string uploadPath)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(stream))
            {
                // 1920 x 1080 boyut sınırı
                var maxWidth = 1920;
                var maxHeight = 1080;

                // Oranları koruyarak yeniden boyutlandır
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, maxHeight)
                }));

                // JPEG formatında sıkıştırılmış kaydet (istenirse kalite verilebilir)
                await image.SaveAsync(filePath, new JpegEncoder { Quality = 85 });
            }

            return "/uploads/gallery/" + fileName;
        }

        public static async Task<string> SaveImage(IFormFile file, string folder, IWebHostEnvironment _environment, ILogger<ActivitiesWizardController> _logger)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                {
                    throw new Exception("Geçersiz dosya formatı. Sadece JPG, JPEG, PNG ve GIF dosyaları yüklenebilir.");
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Görseli okuma ve yeniden boyutlandırma
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream()))
                {
                    var resizeWidth = 1920;
                    var resizeHeight = 1080;

                    // Mevcut boyut 1920x1080'den küçükse yeniden boyutlandırma yapma
                    if (image.Width > resizeWidth || image.Height > resizeHeight)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(resizeWidth, resizeHeight)
                        }));
                    }

                    await image.SaveAsync(filePath);
                }

                return $"/uploads/{folder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya yüklenirken hata oluştu");
                throw new Exception("Dosya yüklenirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
