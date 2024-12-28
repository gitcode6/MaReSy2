
using System.Drawing;
using MaReSy2_Api.Models;
using MaReSy2_Api.Services.ImageService;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public class ImageUploadService : IImageUploadService
    {
        public async Task<byte[]> ValidateAndProcessImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException($"Es wurde keine gültige Bilddatei hochgeladen");
            }

            if (image.ContentType != "image/jpeg")
            {
                throw new ArgumentException("Nur JPG/JPEG Dateien sind erlaubt.");
            }

            if (image.Length > 8 * 1024 * 1024)
            {
                throw new ArgumentException($"Die Datei darf nicht größer als 8MB sein.");

            }

            var extension = Path.GetExtension(image.FileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg")
            {
                throw new ArgumentException("Nur JPG/JPEG Dateien sind erlaubt.");

            }



            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);

                try
                {
                    using var img = Image.FromStream(ms);
                    if (img.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
                    {
                        throw new ArgumentException("Nur JPG/JPEG Dateien sind erlaubt.");

                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Die Datei ist kein gültiges Bild!");
                }

                ms.Position = 0; //Stream zurücksetzen

                return ms.ToArray();
            }
        }
    }
}
