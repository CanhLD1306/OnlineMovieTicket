using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ITicketRepository
    {
        Task<(IEnumerable<Ticket>? ticket, int totalCount, int filterCount)> GetTicketsAsync
        (string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber, 
        int pageSize, 
        string sortBy, 
        bool isDescending);
        Task<(IEnumerable<Ticket>? tickets, int totalCount)> GetTicketsByUser(Guid userId, int maxRecord, bool? isUpcoming);
        Task CreateTicketsAsync(IEnumerable<Ticket> tickets);
        Task<decimal> GetTotalRevenueThisMonthAsync();
        Task<int> GetTotalTickets();

        Task<List<RevenueByTime>> GetRevenueByDateGroupAsync(string groupBy);
        Task<List<TicketRatioBySeatType>> GetTicketRatioBySeatTypeAsync();
    }
}