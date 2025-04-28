using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;

namespace OnlineMovieTicket.BL.DTOs.Payment
{
    public class CreatePaymentIntentRequest
    {
        public decimal Amount { get; set; }
    }
}