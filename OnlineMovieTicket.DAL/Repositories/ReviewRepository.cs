using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetReviewByUserAsync(Guid userId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.CreatedBy == userId && !r.IsDeleted);
                                        
            return review;
        }
        public async Task<IEnumerable<Review>?> GetReviewsAsync(long movieId, int maxRecord)
        {
            var reviews = await _context.Reviews
                                        .Where(r => r.MovieId == movieId)
                                        .OrderByDescending(r => r.UpdatedAt)
                                        .Take(maxRecord)
                                        .ToListAsync();
            return reviews;
        }

        public async Task AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
