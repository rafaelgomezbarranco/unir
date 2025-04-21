namespace NotificationWebApi.Business.WhatApps;

public interface IWhatAppNotification
{
    Task<bool> SendWhatApp(string receiverPhoneNumber, string notificationMessage);
}