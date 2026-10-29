using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Models;
using TestWebApp.Services;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notification;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, INotificationService notification)
        {
            _logger = logger;
            _configuration = configuration;
            _notification = notification;
        }

        public IActionResult Index()
        {
            ViewBag.Connection = _configuration["ConnectionStrings"];
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(string email)
        {
            string emailResult = _notification.SendEmail(email, "Test Subject", "Test Body");
            ViewBag.EmailResult = emailResult;
            return View();
        }

        [HttpPost("send-otp")]
        public IActionResult SendOtp(string phoneNumber)
        {
            string smsResult = _notification.SendSms(phoneNumber, "Your OTP is 123456");
            ViewBag.SmsResult = smsResult;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
