using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Ticket;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ITicketService _ticketService;
        public OrderController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetTickets")]
        public async Task<IActionResult> GetTickets([FromForm] TicketQueryDTO queryModel)
        {
            var result = await _ticketService.GetTickesAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Tickets
            });
        }
    }
}