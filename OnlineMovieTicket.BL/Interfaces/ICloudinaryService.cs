using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.BL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICloudinaryService
    {
        Task<Response<string>> UploadBannerAsync(IFormFile file);
        Task<Response<string>> UploadMovieBannerAsync(IFormFile file);
        Task<Response<string>> UploadMoviePosterAsync(IFormFile file);
        Task<Response<string>> UploadProfileAsync(IFormFile file);
    }
}
