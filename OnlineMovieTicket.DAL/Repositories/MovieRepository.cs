﻿using Microsoft.EntityFrameworkCore;
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
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Movie>? movies, int totalCount, int filterCount)> GetMoviesAsync(
            string? searchTerm, 
            DateTime? startDate, 
            DateTime? endDate, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Movies
                                .Where(m => !m.IsDeleted)
                                .AsQueryable();
                                
            var totalCount = await query.CountAsync();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(m => m.Title.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));
            }

            if (startDate.HasValue)
            {
                query = query.Where(m => m.ReleaseDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(m => m.ReleaseDate <= endDate);
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            var filterCount = await query.CountAsync();

            var movies = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (movies, totalCount, filterCount);
        }

        public async Task<Movie?> GetMovieByIdAsync(long movieId)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == movieId && !m.IsDeleted);
        }

        public async Task<Movie?> GetMovieByTitleAsync(long movieId, string title)
        {
            if(movieId > 0)
            {
                return await _context.Movies
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            m => m.Id != movieId 
                                            && m.Title.Replace(" ", "").ToLower() == title.Replace(" ", "").ToLower()
                                            && !m.IsDeleted);
            }
            return await _context.Movies
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            m => m.Title.Replace(" ", "").ToLower() == title.Replace(" ", "").ToLower()
                                            && !m.IsDeleted);
        }

        public async Task CreateMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Movie>?> GetAllMoviesAsync()
        {
            return await _context.Movies
                                .Where(m => !m.IsDeleted)
                                .ToListAsync();
        }

        public async Task<(IEnumerable<Movie>? movies, int totalCount, int filterCount)> GetMoviesForUserAsync(
            string? searchTerm, 
            DateTime? startDate, 
            DateTime? endDate, 
            bool? IsComingSoon, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Movies
                                .Where(m => !m.IsDeleted)
                                .AsQueryable();

            var totalCount = await query.CountAsync();              
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(m => m.Title.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));
            }

            if (startDate.HasValue)
            {
                query = query.Where(m => m.ReleaseDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(m => m.ReleaseDate <= endDate);
            }

            if (IsComingSoon.HasValue)
            {
                if(IsComingSoon.Value)
                {
                    query = query.Where(m => m.ReleaseDate > DateTime.Now);
                }
                else
                {
                    query = query.Where(m => m.ReleaseDate <= DateTime.Now);
                }
            }
            var filterCount = await query.CountAsync();

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));


            var movies = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (movies, totalCount, filterCount);
        }

        public async Task<(List<TopMovieRevenueModel>? movies, int totalCount, int filterCount)> GetTop5MoviesByRevenueAsync()
        {
            var movies = await _context.Movies
                                        .Where(m => !m.IsDeleted)
                                        .Select(m => new TopMovieRevenueModel
                                        {
                                            Title = m.Title,
                                            PosterURL = m.PosterURL,
                                            ReleaseDate = m.ReleaseDate,
                                            TotalRevenue = m.Showtimes
                                                .Where(s => !s.IsDeleted)
                                                .SelectMany(s => s.ShowtimeSeats)
                                                .Where(ss => !ss.IsDeleted)
                                                .SelectMany(ss => ss.Tickets)
                                                .Where(t => !t.IsDeleted && t.IsPaid)
                                                .Sum(t => t.Price),
                                            TotalTicketSold = m.Showtimes
                                                .Where(s => !s.IsDeleted)
                                                .SelectMany(s => s.ShowtimeSeats)
                                                .Where(ss => !ss.IsDeleted)
                                                .SelectMany(ss => ss.Tickets)
                                                .Count(t => !t.IsDeleted && t.IsPaid)
                                        })
                                        .OrderByDescending(m => m.TotalRevenue)
                                        .Take(5)
                                        .ToListAsync();

            var totalCount = movies.Count();  
            var filterCount = movies.Count();

            return (movies, totalCount, filterCount);
        }
    }
}
