using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Dashboard;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly IShowtimeService _showtimeService;
        private readonly IMovieService _movieService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IMovieService movieService,
            ITicketService ticketService,
            IUserService userService,
            IShowtimeService showtimeService,
            ILogger<DashboardController> logger
        )
        {
            _movieService = movieService;
            _ticketService = ticketService;
            _userService = userService;
            _showtimeService = showtimeService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetOverviewStatistics")]
        public async Task<IActionResult> GetOverviewStatistics()
        {
            var totalRevenue = await _ticketService.GetTotalRevenueThisMonthAsync();
            var totalTicketsSold = await _ticketService.GetTotalTickets();
            var totalActiveShowtimes = await _showtimeService.GetTotalShowtimes();
            var totalCustomers = await _userService.GetTotalCustomers();

            var result = new OverviewStatisticsDTO{
                TotalRevenue = totalRevenue,
                TotalTicketsSold = totalTicketsSold,
                TotalActiveShowtimes = totalActiveShowtimes,
                TotalCustomers = totalCustomers
            };

            return PartialView("_OverviewStatisticsCards", result);
        }

        [HttpPost("GetTop5Movies")]
        public async Task<IActionResult> GetTop5Movies()
        {
            var result = await _movieService.GetTop5MoviesByRevenueAsync();
            return Json(new
            {
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.MovieRevenuesDTO
            });
        }

        [HttpGet("GetRevenueByDateGroup")]
        public async Task<IActionResult> GetRevenueByDateGroup(string groupBy)
        {
            var result = await _ticketService.GetRevenueByDateGroupAsync(groupBy);
            return Json(result);
        }

        [HttpGet("GetTicketRatioBySeatType")]
        public async Task<IActionResult> GetTicketRatioBySeatType()
        {
            var result = await _ticketService.GetTicketRatioBySeatTypeAsync();
            return Json(result);
        }
    }
}
