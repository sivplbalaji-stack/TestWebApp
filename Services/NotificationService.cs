namespace TestWebApp.Services
{
    public class NotificationService : INotificationService
    {
        public string SendEmail(string to, string subject, string body)
        {
            return $"Email sent to {to} with subject '{subject}' and body '{body}'.";
        }

        public string SendPushNotification(string deviceId, string message)
        {
            return $"Push notification sent to device {deviceId} with message '{message}'.";
        }

        public string SendSms(string phoneNumber, string message)
        {
            return $"SMS sent to {phoneNumber} with message '{message}'.";
        }
    }
}
