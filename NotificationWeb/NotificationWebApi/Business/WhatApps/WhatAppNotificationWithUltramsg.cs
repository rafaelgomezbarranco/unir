namespace NotificationWebApi.Business.WhatApps;

public class WhatAppNotificationWithUltramsg : IWhatAppNotification
{
    public Task<bool> SendWhatApp(string receiverPhoneNumber, string notificationMessage)
    {
        throw new NotImplementedException();
    }
}