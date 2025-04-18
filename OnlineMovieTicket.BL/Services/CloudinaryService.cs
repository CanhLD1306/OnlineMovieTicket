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
        public async Task<Response<string>> UploadBannerAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "Banner",
                Transformation = new Transformation().Width(1600).Height(600).Crop("fill").Gravity("auto")
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));

            if (uploadResult.Error != null)
            {
                return new Response<string>( false, "Upload image fail!", null );
            }

            return new Response<string>(true, "Upload image successful", uploadResult.SecureUrl.ToString());

        }

        public async Task<Response<string>> UploadMovieBannerAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "MovieBanner",
                Transformation = new Transformation().Width(1600).Height(600).Crop("fill").Gravity("auto")
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));

            if (uploadResult.Error != null)
            {
                return new Response<string>( false, "Upload image fail!", null );
            }

            return new Response<string>(true, "Upload image successful", uploadResult.SecureUrl.ToString());
        }

        public async Task<Response<string>> UploadMoviePosterAsync(IFormFile file)
        {
           using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "MoviePoster",
                Transformation = new Transformation().Width(500).Height(750).Crop("fill").Gravity("auto")
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));

            if (uploadResult.Error != null)
            {
                return new Response<string>( false, "Upload image fail!", null );
            }

            return new Response<string>(true, "Upload image successful", uploadResult.SecureUrl.ToString());
        }

        public async Task<Response<string>> UploadProfileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "Profile",
                Transformation = new Transformation().Width(150).Height(150).Crop("fill").Gravity("auto")
            };

            var uploadResult = await Task.Run(() => _cloudinary.Upload(uploadParams));

            if (uploadResult.Error != null)
            {
                return new Response<string>( false, "Upload image fail!", null );
            }

            return new Response<string>(true, "Upload image successful", uploadResult.SecureUrl.ToString());
        }
    }
}
