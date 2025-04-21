namespace NotificationWebApi.Business.SMSs;

public interface ISMSNotification
{
    Task<bool> SendSMS(string receiverPhoneNumber, string notificationMessage);
}