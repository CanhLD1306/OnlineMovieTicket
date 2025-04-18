using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Migrations;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetRooms")]
        public async Task<IActionResult> GetRooms(RoomQueryDTO queryModel)
        {
            var result = await _roomService.GetRoomsAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Rooms
            });
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new RoomWithSeatsDTO();
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] RoomWithSeatsDTO roomWithSeats, [FromForm] string SeatsJson)
        {
            try
            {
                var seatList = JsonConvert.DeserializeObject<List<SeatDTO>>(SeatsJson);
                roomWithSeats.Seats = seatList ?? new List<SeatDTO>();
            }
            catch (Exception)
            {
                return Json(new {success = false, message = "Invalid seats data."});
            }
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _roomService.CreateRoomAsync(roomWithSeats);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long roomId)
        {
            var result = await _roomService.GetRoomWithSeatsById(roomId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return View(result.Data);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] RoomWithSeatsDTO roomWithSeats, [FromForm] string SeatsJson)
        {
            try
            {
                var seatList = JsonConvert.DeserializeObject<List<SeatDTO>>(SeatsJson);
                roomWithSeats.Seats = seatList ?? new List<SeatDTO>();
            }
            catch (Exception)
            {
                return Json(new {success = false, message = "Invalid seats data."});
            }
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _roomService.UpdateRoomsAsync(roomWithSeats);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _roomService.GetRoomsByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_DeleteRoom", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] RoomDTO room)
        {
            var result = await _roomService.DeleteRoomAsync(room.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(long id)
        {
            var result = await _roomService.ChangeStatusAsync(id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}