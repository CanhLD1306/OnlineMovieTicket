using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>?> GetAllMoviesAsync();
        Task<(IEnumerable<Movie>? movies, int totalCount, int filterCount)> GetMoviesForUserAsync(
            string? searchTerm, 
            DateTime? startDate, 
            DateTime? endDate,
            bool? IsComingSoon,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending
        );
        Task<(IEnumerable<Movie>? movies, int totalCount, int filterCount)> GetMoviesAsync(
            string? searchTerm, 
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<Movie?> GetMovieByIdAsync(long movieId);
        Task<Movie?> GetMovieByTitleAsync(long movieId, string title);
        Task CreateMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
    }
}
