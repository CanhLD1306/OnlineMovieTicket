using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Movie;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IMovieService
    {

        Task<IEnumerable<MovieDTO>?> GetAllMoviesAsync();
        Task<MoviesList> GetMoviesForUserAsync(MovieQueryForUserDTO queryDTO);
        Task<MoviesList> GetMoviesAsync(MovieQueryDTO queryDTO);
        Task<Response<MovieDTO>> GetMovieByIdAsync(long movieId);
        Task<Response> CreateMovieAsync(MovieDTO movieDTO);
        Task<Response> UpdateMovieAsync(MovieDTO movieDTO);
        Task<Response> DeleteMovieAsync(long movieId);
    }
}
