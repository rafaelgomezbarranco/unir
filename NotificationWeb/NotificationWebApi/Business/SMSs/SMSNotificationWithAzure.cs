namespace NotificationWebApi.Business.SMSs;

public class SMSNotificationWithAzure : ISMSNotification
{
    public Task<bool> SendSMS(string receiverPhoneNumber, string notificationMessage)
    {
        throw new NotImplementedException();
    }
}