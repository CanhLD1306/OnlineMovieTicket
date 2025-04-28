using System.Transactions;
using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.BL.DTOs.Ticket;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IShowtimeSeatService _showtimeSeatService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public TicketService(
            IShowtimeSeatService showtimeSeatService,
            ITicketRepository ticketRepository, 
            ICityRepository cityRepository,
            IAuthService authService, 
            IMapper mapper
        )
        {
            _showtimeSeatService = showtimeSeatService;
            _ticketRepository = ticketRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<Response> CreateTicketsAsync(List<ShowtimeSeatDTO> showtimeSeatDTOs, decimal price)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var tickets = new List<Ticket>();
                    foreach(var showtimeSeatDTO in showtimeSeatDTOs)
                    {
                        var ticket= new Ticket();
                        ticket.TicketCode = GenerateTicketCode();
                        ticket.ShowtimeSeatId = showtimeSeatDTO.Id;
                        ticket.Price = showtimeSeatDTO.PriceMultiplier * price;
                        ticket.IsPaid = true;
                        ticket.PurchaseDate = DateTime.Now;
                        ticket.UserId = (await _authService.GetUserId()).Data.ToString();
                        ticket.CreatedAt = DateTime.UtcNow;
                        ticket.CreatedBy = (await _authService.GetUserId()).Data;
                        ticket.UpdatedAt = DateTime.UtcNow;
                        ticket.UpdatedBy = (await _authService.GetUserId()).Data;
                        ticket.IsDeleted = false;

                        var result = await _showtimeSeatService.BookShowtimeSeatAsync(showtimeSeatDTO.Id);
                        if(!result.Success)
                        {
                            return new Response(false, result.Message);
                        }
                        tickets.Add(ticket);
                    }
                    await _ticketRepository.CreateTicketsAsync(tickets);
                    scope.Complete();
                    return new Response(true, "Create Tickets successfull");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return new Response(false, "Create Tickets failed: " + ex.InnerException.Message);
                    }
                    return new Response(false, "Create Tickets failed: " + ex.Message);
                }
            }

        }

        public async Task<ListTicketsDTO> GetTickesAsync(TicketQueryDTO queryDTO)
        {
            var (tickets, totalCount, filterCount) = await _ticketRepository.GetTicketsAsync(
                                                                            queryDTO.SearchTerm,
                                                                            queryDTO.StartDate,
                                                                            queryDTO.EndDate,
                                                                            queryDTO.PageNumber,
                                                                            queryDTO.PageSize,
                                                                            queryDTO.SortBy,
                                                                            queryDTO.IsDescending);
            var ticketsDTO = _mapper.Map<List<TicketDTO>>(tickets);
            return new ListTicketsDTO{
                Tickets = ticketsDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<ListTicketForUser> GetTicketsForUser(int maxRecord, bool? isUpcoming)
        {
            var (tickets, totalCount) = await _ticketRepository.GetTicketsByUser(
                                                (await _authService.GetUserId()).Data,
                                                maxRecord,
                                                isUpcoming);
            var ticketsDTO = _mapper.Map<List<TicketForUserDTO>>(tickets);
            return new ListTicketForUser{
                Tickets = ticketsDTO,
                TotalCount = totalCount
            };
        }

        private string GenerateTicketCode(int length = 4)
        {
            string datePart = DateTime.Now.ToString("yyyyMMddHHmmss"); 

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string randomPart = new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());

            return $"OMT-{datePart}-{randomPart}";
        }
    }
}