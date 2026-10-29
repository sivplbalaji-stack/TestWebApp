using Microsoft.AspNetCore.Mvc;
using TestWebApp.Services;

namespace TestWebApp.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IPaymentService _paymentService;

        public SubscriptionController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            string result = _paymentService.Pay(1000);
            ViewBag.PaymentResult = result;
            return View();
        }
    }
}
