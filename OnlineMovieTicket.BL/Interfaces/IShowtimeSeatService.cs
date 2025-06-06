﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IShowtimeSeatService
    {
        Task<IEnumerable<ShowtimeSeatDTO>?> GetAllShowtimeSeatsByShowtimeAsync(long showtimeId);
        Task<Response> CreateShowtimeSeatsAsync(IEnumerable<Seat> seats, long showtimeId);
        Task<Response> DeleteShowtimeSeatsByRoomAsync(long showtimeId);
        Task<Response> BookShowtimeSeatAsync(long showtimeSeatId);
    }
}
