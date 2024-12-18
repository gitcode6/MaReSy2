namespace MaReSy2_Api.Services
{
    public interface IImageUploadService
    {
        Task<byte[]> ValidateAndProcessImageAsync(IFormFile image);
    }
}
