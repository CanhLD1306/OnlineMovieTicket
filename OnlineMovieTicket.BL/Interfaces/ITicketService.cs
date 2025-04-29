using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Dashboard;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.BL.DTOs.Ticket;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ITicketService
    {

        Task<Response> CreateTicketsAsync(List<ShowtimeSeatDTO> showtimeSeatDTOs, decimal price);
        Task<ListTicketForUser> GetTicketsForUser(int maxRecord, bool? isUpcoming);
        Task<ListTicketsDTO> GetTickesAsync(TicketQueryDTO queryDTO);

        Task<decimal> GetTotalRevenueThisMonthAsync();
        Task<int> GetTotalTickets();
        Task<List<RevenueByTimeDTO>> GetRevenueByDateGroupAsync(string groupBy);
        Task<List<TicketRatioBySeatTypeDTO>> GetTicketRatioBySeatTypeAsync();
    }
}