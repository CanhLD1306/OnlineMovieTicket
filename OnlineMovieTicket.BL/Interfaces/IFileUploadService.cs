using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.BL.DTOs;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IFileUploadService
    {
        Task<Response> ValidateImageFile(IFormFile file);
    }
}