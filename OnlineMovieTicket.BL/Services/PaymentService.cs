using Microsoft.Extensions.Configuration;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Payment;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Configurations;
using Stripe;

namespace OnlineMovieTicket.BL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly ITicketService _ticketService;
        public PaymentService(IConfiguration configuration, ITicketService ticketService)
        {
            _stripeSettings = configuration.GetSection("StripeSettings").Get<StripeSettings>();
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _ticketService = ticketService;
            
        }

        public async Task<Response> CompletePaymentAsync(CompletePaymentRequest request)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(request.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    var result = await _ticketService.CreateTicketsAsync(request.ShowtimeSeatDTOs, request.Price);
                    if(!result.Success){
                        return new Response(false, result.Message);
                    }
                    return new Response(true, "Payment successful");
                }else if (paymentIntent.Status == "requires_action" || paymentIntent.Status == "requires_payment_method")
                {
                    return new Response(false, "Payment requires further authentication");
                }

                return new Response(false, "Payment fail");
            }
            catch (StripeException ex)
            {
                return new Response(false, "Payment fail " + ex.Message);
            }
        }

        public async Task<PaymentIntent> CreatePaymentIntent(CreatePaymentIntentRequest request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent;
        }
    }
}    