namespace MaReSy2_Api.Services.ImageService
{
    public interface IImageUploadService
    {
        Task<byte[]> ValidateAndProcessImageAsync(IFormFile image);
    }
}
