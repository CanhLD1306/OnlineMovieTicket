using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.BL.Services
{
    public class FileUploadService : IFileUploadService
    {
        public Task<Response> ValidateImageFile(IFormFile file)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp"};
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!validExtensions.Contains(fileExtension))
            {
                return Task.FromResult(new Response(false, "Invalid file format. Only upload file image!"));
            }

            return Task.FromResult(new Response(true, null));

        }
    }
}
