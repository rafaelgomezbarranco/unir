using Azure.Communication.Sms;
using Microsoft.Extensions.Options;

namespace NotificationWebApi.Business.SMSs;

public class SMSNotificationWithAzure : ISMSNotification
{
    private readonly AzureSmsSettings _smsSettings;

    public SMSNotificationWithAzure(IOptions<AzureSmsSettings> smsOptions)
    {
        _ = smsOptions ?? throw new ArgumentNullException(nameof(smsOptions));
        _smsSettings = smsOptions.Value ?? throw new ArgumentNullException(nameof(smsOptions.Value));
    }

    public async Task<bool> SendSMS(string receiverPhoneNumber, string notificationMessage)
    {
        var smsClient = new SmsClient(_smsSettings.ConnectionString);

        var response = await smsClient.SendAsync(
            from: _smsSettings.SenderPhoneNumber,
            to: receiverPhoneNumber,
            message: notificationMessage
        );

        return (bool)response?.Value?.Successful;
    }
}