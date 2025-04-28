using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;

namespace OnlineMovieTicket.BL.DTOs.Payment
{
    public class CompletePaymentRequest
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public List<ShowtimeSeatDTO> ShowtimeSeatDTOs { get; set; } = new List<ShowtimeSeatDTO>();
        public decimal Price { get; set; }
    }
}