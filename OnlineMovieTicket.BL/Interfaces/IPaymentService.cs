using Stripe;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.DTOs.Payment;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntent(CreatePaymentIntentRequest request);
        Task<Response> CompletePaymentAsync(CompletePaymentRequest request);
    }
}
