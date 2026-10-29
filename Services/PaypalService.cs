namespace TestWebApp.Services
{
    public class PaypalService : IPaymentService
    {
        public string Pay(decimal amount)
        {
            return $"Paid {amount} using PayPal.";
        }
    }
}
