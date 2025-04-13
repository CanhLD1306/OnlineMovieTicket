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
        Task<Response<string>> UploadImageAsync(IFormFile file, string folder);
    }
}
