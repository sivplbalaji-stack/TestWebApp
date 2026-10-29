namespace TestWebApp.Services
{
    public interface INotificationService
    {
        string SendEmail(string to, string subject, string body);
        string SendSms(string phoneNumber, string message);
        string SendPushNotification(string deviceId, string message);
    }
}
