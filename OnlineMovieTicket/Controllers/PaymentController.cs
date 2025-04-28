using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Payment;
using OnlineMovieTicket.BL.Interfaces;
using Stripe;

namespace OnlineMovieTicket.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("CreatePaymentIntent")]
        public async Task<JsonResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                var paymentIntent = await _paymentService.CreatePaymentIntent(request);


                return Json(new
                {
                    success = true,
                    clientSecret = paymentIntent.ClientSecret
                });
            }
            catch (StripeException ex)
            {
                _logger.LogError("Stripe error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal Server Error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    error = "Internal Server Error: " + ex.Message
                });
            }
        }

        [HttpPost("CompletePayment")]
        public async Task<JsonResult> CompletePayment([FromBody] CompletePaymentRequest request)
        {
            try
            {
                var response = await _paymentService.CompletePaymentAsync(request);

                if (response.Success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Payment completed successfully!"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = response.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Complete payment error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Internal Server Error: " + ex.Message
                });
            }
        }
    }
}
