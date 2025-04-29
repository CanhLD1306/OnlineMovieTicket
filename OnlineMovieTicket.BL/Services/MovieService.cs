using System.Text.RegularExpressions;
using System.Transactions;
using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Movie;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.BLL.DTOs.Dashboard;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public MovieService(
            IMovieRepository movieRepository, 
            IShowtimeRepository showtimeRepository,
            IFileUploadService fileUploadService,
            ICloudinaryService cloudinaryService,
            IAuthService authService, 
            IMapper mapper
)
        {
            _movieRepository = movieRepository;
            _showtimeRepository = showtimeRepository;
            _fileUploadService = fileUploadService;
            _cloudinaryService = cloudinaryService;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<Response> CreateMovieAsync(MovieDTO movieDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(!await _authService.IsAdminAsync()){
                        return new Response(false, "You do not have permissions");
                    }
                    if(await _movieRepository.GetMovieByTitleAsync(movieDTO.Id, movieDTO.Title) != null)
                    {
                        return new Response(false, "Movie title already exists!");
                    }
                    if (DateOnly.FromDateTime(movieDTO.ReleaseDate) < DateOnly.FromDateTime(DateTime.Today.AddDays(5)))
                    {
                        return new Response(false, "Please select a release date that is at least 5 days from today.");
                    }
                    if (movieDTO.PosterFile == null)
                    {
                        return new Response(false, "Please upload poster!");
                    }
                    if (movieDTO.BannerFile == null)
                    {
                        return new Response(false, "Please upload banner!");
                    }
                    if(movieDTO.TrailerURL == null)
                    {
                        return new Response(false, "Please upload trailer!");
                    }
                    if (!IsValidYouTubeUrl(movieDTO.TrailerURL))
                    {
                        return new Response(false, "Please enter a valid YouTube URL.");
                    }

                    var validatePosterResult = await _fileUploadService.ValidateImageFile(movieDTO.PosterFile);

                    if(!validatePosterResult.Success)
                    {
                        return new Response(false, validatePosterResult.Message);
                    }

                    var validateBannerResult = await _fileUploadService.ValidateImageFile(movieDTO.BannerFile);

                    if(!validateBannerResult.Success)
                    {
                        return new Response(false, validateBannerResult.Message);
                    }

                    var uploadPosterResult = await _cloudinaryService.UploadMoviePosterAsync(movieDTO.PosterFile);

                    if (!uploadPosterResult.Success)
                    {
                        return new Response(false, uploadPosterResult.Message);
                    }

                    var uploadBannerResult = await _cloudinaryService.UploadMovieBannerAsync(movieDTO.BannerFile);

                    if (!uploadBannerResult.Success)
                    {
                        return new Response(false, uploadBannerResult.Message);
                    }

                    var movie = _mapper.Map<Movie>(movieDTO);
                    movie.PosterURL = uploadPosterResult.Data!;
                    movie.BannerURL = uploadBannerResult.Data!;
                    movie.CreatedAt = DateTime.UtcNow;
                    movie.CreatedBy = (await _authService.GetUserId()).Data;
                    movie.UpdatedAt = DateTime.UtcNow;
                    movie.UpdatedBy = (await _authService.GetUserId()).Data;
                    movie.IsDeleted = false;

                    await _movieRepository.CreateMovieAsync(movie);
                    scope.Complete();
                    return new Response(true, "Add new movie successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add new movie fail!");
                }
            }
        }

        public async Task<Response> DeleteMovieAsync(long movieId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(!await _authService.IsAdminAsync()){
                        return new Response(false, "You do not have permissions");
                    }
                    var movie = await _movieRepository.GetMovieByIdAsync(movieId);
                    if(movie == null){
                        return new Response(false, "Movie not found");
                    }

                    if(await _showtimeRepository.MovieHasFutureShotime(movieId))
                    {
                        return new Response(false, "Movie has future showtime, cannot delete!");
                    }
                    
                    movie.IsDeleted = true;
                    movie.UpdatedAt = DateTime.UtcNow;
                    movie.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _movieRepository.UpdateMovieAsync(movie);
                    scope.Complete();
                    return new Response(true, "Delete movie successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete movie fail!");
                }
            }
        }

        public async Task<IEnumerable<MovieDTO>?> GetAllMoviesAsync()
        {
            var movies = await _movieRepository.GetAllMoviesAsync();
            return _mapper.Map<IEnumerable<MovieDTO>>(movies);
        }

        public async Task<Response<MovieDTO>> GetMovieByIdAsync(long movieId)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if(movie != null){
                var movieDTO = _mapper.Map<MovieDTO>(movie);
                return new Response<MovieDTO>(true, null,movieDTO);
            }
            return new Response<MovieDTO>(false,"Movie not found");
        }

        public async Task<MoviesList> GetMoviesAsync(MovieQueryDTO queryDTO)
        {
            var (movies, totalCount, filterCount) = await _movieRepository.GetMoviesAsync(
                                                                    queryDTO.SearchTerm,
                                                                    queryDTO.StartDate,
                                                                    queryDTO.EndDate,
                                                                    queryDTO.PageNumber,
                                                                    queryDTO.PageSize,
                                                                    queryDTO.SortBy,
                                                                    queryDTO.IsDescending
                                                                );
            var moviesDTO = _mapper.Map<IEnumerable<MovieDTO>>(movies);

            return new MoviesList
            {
                Movies = moviesDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }
        public async Task<MoviesList> GetMoviesForUserAsync(MovieQueryForUserDTO queryDTO)
        {
            var (movies, totalCount, filterCount) = await _movieRepository.GetMoviesForUserAsync(
                                                                    queryDTO.SearchTerm,
                                                                    queryDTO.StartDate,
                                                                    queryDTO.EndDate,
                                                                    queryDTO.IsCommingSoon,
                                                                    queryDTO.PageNumber,
                                                                    queryDTO.PageSize,
                                                                    queryDTO.SortBy,
                                                                    queryDTO.IsDescending
                                                                );
            var moviesDTO = _mapper.Map<IEnumerable<MovieDTO>>(movies);

            return new MoviesList
            {
                Movies = moviesDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<ListMovieRevenuesDTO> GetTop5MoviesByRevenueAsync()
        {
            
            var (movies, totalCount, filterCount) = await _movieRepository.GetTop5MoviesByRevenueAsync();
            
            var moviesDTO = _mapper.Map<List<MovieRevenueDTO>>(movies);
            return new ListMovieRevenuesDTO{
                MovieRevenuesDTO = moviesDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };

        }

        public async Task<Response> UpdateMovieAsync(MovieDTO movieDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(!await _authService.IsAdminAsync()){
                        return new Response(false, "You do not have permissions");
                    }
                    var movie = await _movieRepository.GetMovieByIdAsync(movieDTO.Id);
                    if(movie == null){
                        return new Response(false, "Movie not found");
                    }
                    if (await _movieRepository.GetMovieByTitleAsync(movie.Id, movieDTO.Title) != null)
                    {
                        return new Response(false, "Movie title already exists!");
                    }
                    if (DateOnly.FromDateTime(movieDTO.ReleaseDate) < DateOnly.FromDateTime(movie.ReleaseDate))
                    {
                        return new Response(false, "The new release date cannot be earlier than the original release date.");
                    }
                    if (!IsValidYouTubeUrl(movieDTO.TrailerURL!))
                    {
                        return new Response(false, "Please enter a valid YouTube URL.");
                    }

                    _mapper.Map(movieDTO, movie);
                    if (movieDTO.PosterFile != null)
                    {
                        var validatePosterResult = await _fileUploadService.ValidateImageFile(movieDTO.PosterFile!);

                        if(!validatePosterResult.Success)
                        {
                            return new Response(false, validatePosterResult.Message);
                        }
                        var result = await _cloudinaryService.UploadMoviePosterAsync(movieDTO.PosterFile!);
                        if (!result.Success)
                        {
                            return new Response(false, result.Message);
                        }
                        movie.PosterURL = result.Data!;
                    }

                    if (movieDTO.BannerFile != null)
                    {
                        var validateBannerResult = await _fileUploadService.ValidateImageFile(movieDTO.BannerFile!);

                        if(!validateBannerResult.Success)
                        {
                            return new Response(false, validateBannerResult.Message);
                        }
                        var result = await _cloudinaryService.UploadMoviePosterAsync(movieDTO.BannerFile!);
                        if (!result.Success)
                        {
                            return new Response(false, result.Message);
                        }
                        movie.BannerURL = result.Data!;
                    }

                    movie.UpdatedAt = DateTime.UtcNow;
                    movie.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _movieRepository.UpdateMovieAsync(movie);
                    scope.Complete();
                    return new Response(true, "Update movie successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Update movie fail!");
                }
            }
        }

        private bool IsValidYouTubeUrl(string url)
        {
            var youtubeRegex = new Regex(@"^(?:https?:\/\/)?(?:www\.)?(youtube\.com\/watch\?v=|youtu\.be\/)([^\s&]{11})$", RegexOptions.IgnoreCase);
            return youtubeRegex.IsMatch(url);
        }
    }
}
