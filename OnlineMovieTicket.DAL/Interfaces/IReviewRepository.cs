using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetReviewByUserAsync(Guid userId);
        Task<IEnumerable<Review>?> GetReviewsAsync(long movieId, int maxRecord);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
    }
}
