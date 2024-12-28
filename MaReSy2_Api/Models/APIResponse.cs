using Microsoft.Build.Framework;

namespace MaReSy2_Api.Models
{
    public class APIResponse<T>
    {
        public T? Data { get; set; }
        public bool Success => Errors == null || !Errors.Any();
        public List<ErrorDetail> Errors { get; set; } = new();
        public int? StatusCode { get; set; }

        public string? Message { get; set; }
    }
}
