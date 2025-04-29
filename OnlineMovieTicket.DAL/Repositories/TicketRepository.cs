using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager,ILogger<TicketRepository> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateTicketsAsync(IEnumerable<Ticket> tickets)
        {
            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RevenueByTime>> GetRevenueByDateGroupAsync(string groupBy)
        {
            var now = DateTime.Now;
            var startOfWeek = now.Date.AddDays(-(int)(now.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)now.DayOfWeek - 1)); // Monday
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);
            var query = _context.Tickets.Where(t => t.IsPaid && !t.IsDeleted);

            switch (groupBy.ToLower())
            {
                case "day":
                    var dayStart = startOfWeek;
                    var dayEnd = now.Date;

                    var groupedDayData = await query
                                            .Where(t => t.PurchaseDate.Date >= dayStart && t.PurchaseDate.Date <= dayEnd)
                                            .GroupBy(t => t.PurchaseDate.Date)
                                            .Select(g => new
                                            {
                                                Date = g.Key,
                                                TotalRevenue = g.Sum(x => x.Price)
                                            })
                                            .ToListAsync();

                    var fullDays = Enumerable.Range(0, (dayEnd - dayStart).Days + 1)
                                    .Select(offset => dayStart.AddDays(offset))
                                    .Select(date => new RevenueByTime
                                    {
                                        Label = date.ToString("yyyy-MM-dd"),
                                        TotalRevenue = groupedDayData.FirstOrDefault(g => g.Date == date)?.TotalRevenue ?? 0
                                    })
                                    .ToList();

                    return fullDays;

                case "week":
                    var ticketsWeek = await query
                        .Where(t => t.PurchaseDate >= startOfMonth && t.PurchaseDate <= now)
                        .ToListAsync();

                    var calendar = CultureInfo.InvariantCulture.Calendar;
                    var firstWeekOfMonth = calendar.GetWeekOfYear(startOfMonth, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    var currentWeek = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    var groupedWeekData = ticketsWeek
                        .GroupBy(t => calendar.GetWeekOfYear(t.PurchaseDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                        .Select(g => new
                        {
                            Week = g.Key,
                            TotalRevenue = g.Sum(x => x.Price)
                        })
                        .ToList();

                    var fullWeeks = Enumerable.Range(firstWeekOfMonth, currentWeek - firstWeekOfMonth + 1)
                        .Select(w => new RevenueByTime
                        {
                            Label = $"Week {w}",
                            TotalRevenue = groupedWeekData.FirstOrDefault(g => g.Week == w)?.TotalRevenue ?? 0
                        })
                        .ToList();

                    return fullWeeks;

                case "month":
                    var groupedData = await query
                        .Where(t => t.PurchaseDate.Year == now.Year)
                        .GroupBy(t => t.PurchaseDate.Month)
                        .Select(g => new
                        {
                            Month = g.Key,
                            TotalRevenue = g.Sum(x => x.Price)
                        })
                        .ToListAsync();

                    var fullMonths = Enumerable.Range(1, now.Month)
                                        .Select(m => new RevenueByTime
                                        {
                                            Label = $"{now.Year}-{m:00}",
                                            TotalRevenue = groupedData.FirstOrDefault(g => g.Month == m)?.TotalRevenue ?? 0
                                        })
                                        .ToList();

                    return fullMonths;

                default:
                    throw new ArgumentException("Invalid groupBy value. Use 'day', 'week', or 'month'.");
            }
        }

        public async Task<List<TicketRatioBySeatType>> GetTicketRatioBySeatTypeAsync()
        {
            var tickets = await _context.Tickets
                .Where(t => t.IsPaid && !t.IsDeleted)
                .Include(t => t.ShowtimeSeat)
                    .ThenInclude(ss => ss.Seat)
                        .ThenInclude(seat => seat.SeatType)
                .ToListAsync(); // Tải dữ liệu về client

            var result = tickets
                .GroupBy(t => t.ShowtimeSeat.Seat.SeatType)
                .Select(g => new TicketRatioBySeatType
                {
                    Label = g.Key.Name,
                    Value = g.Count(),
                    Color = g.Key.Color
                })
                .ToList();

            return result;
        }

        public async Task<(IEnumerable<Ticket>? ticket, int totalCount, int filterCount)> GetTicketsAsync
        (string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber, 
        int pageSize, 
        string sortBy, 
        bool isDescending)
        {
            var query = _context.Tickets
                                .Include(t => t.ShowtimeSeat)
                                .ThenInclude(ss => ss.Showtime)
                                    .ThenInclude(st => st.Movie)
                                .Include(t => t.User)
                                .Where(t => !t.IsDeleted)
                                .AsQueryable();
            
            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.TicketCode.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()) 
                                || t.ShowtimeSeat.Showtime.Movie.Title.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.PurchaseDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.PurchaseDate <= endDate);
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            var tickets = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            
            int filterCount = await query.CountAsync();

            return (tickets, totalCount, filterCount);
        }

        public async Task<(IEnumerable<Ticket>? tickets, int totalCount)> GetTicketsByUser(Guid userId, int maxRecord, bool? isUpcoming)
        {
            var query = _context.Tickets
                            .Include(t => t.ShowtimeSeat)
                            .ThenInclude(ss => ss.Showtime)
                                .ThenInclude(st => st.Movie)
                            .Include(t => t.ShowtimeSeat)
                            .ThenInclude(ss => ss.Showtime)
                                .ThenInclude(st => st.Room)
                                    .ThenInclude(r => r.Cinema)
                            .Include(t => t.ShowtimeSeat)
                            .ThenInclude(ss => ss.Seat)
                            .Where(t => t.CreatedBy == userId && !t.IsDeleted)
                            .AsQueryable();
            var totalCount = await query.CountAsync();

            if (isUpcoming.HasValue)
            {
                if (isUpcoming.Value)
                {
                    query = query.Where(t => t.ShowtimeSeat.Showtime.StartTime > DateTime.Now);  // Upcoming tickets
                }
                else
                {
                    query = query.Where(t => t.ShowtimeSeat.Showtime.StartTime <= DateTime.Now);  // Past tickets
                }
            }

            var tickets = await query
                                .OrderByDescending(t => t.PurchaseDate)
                                .Take(maxRecord)
                                .ToListAsync();

            return (tickets, totalCount);
        }

        public async Task<decimal> GetTotalRevenueThisMonthAsync()
        {
            var now = DateTime.Now;
            return await _context.Tickets
                            .Where(t => t.IsPaid && !t.IsDeleted
                                    && t.PurchaseDate.Year == now.Year
                                    && t.PurchaseDate.Month == now.Month)
                            .SumAsync(t => t.Price);
        }

        public async Task<int> GetTotalTickets()
        {
            return await _context.Tickets.Where(t => t.IsPaid && !t.IsDeleted).CountAsync();
        }
    }
}