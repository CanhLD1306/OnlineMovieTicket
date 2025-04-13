using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.BL.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using OnlineMovieTicket.DAL.Configurations;
using OnlineMovieTicket.BL.DTOs;


namespace OnlineMovieTicket.BL.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<Response<string>> UploadImageAsync(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Width(1000).Height(1000).Crop("limit")
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));

            if (uploadResult.Error != null)
            {
                return new Response<string>( false, uploadResult.Error.Message, null );
            }

            return new Response<string>(true, "Upload successful", uploadResult.SecureUrl.ToString());

        }
    }
}
