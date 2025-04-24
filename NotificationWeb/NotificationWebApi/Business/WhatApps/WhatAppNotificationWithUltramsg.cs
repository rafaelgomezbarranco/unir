using System.Net;
using Microsoft.Extensions.Options;
using RestSharp;

namespace NotificationWebApi.Business.WhatApps;

public class WhatAppNotificationWithUltramsg : IWhatAppNotification
{
    private readonly UltramsgWhatAppSettings _whatAppSettings;

    public WhatAppNotificationWithUltramsg(IOptions<UltramsgWhatAppSettings> whatAppOptions)
    {
        _ = whatAppOptions ?? throw new ArgumentNullException(nameof(whatAppOptions));
        _whatAppSettings = whatAppOptions.Value ?? throw new ArgumentNullException(nameof(whatAppOptions.Value));
    }

    public async Task<bool> SendWhatApp(string receiverPhoneNumber, string notificationMessage)
    {
        var url = _whatAppSettings.BaseUrl + _whatAppSettings.InstanceId + "/messages/chat";

        var client = new RestClient(url);
        var request = new RestRequest(url, Method.Post);
        request.AddHeader("content-type", "application/x-www-form-urlencoded");
        request.AddParameter("token", _whatAppSettings.Token);
        request.AddParameter("to", receiverPhoneNumber);
        request.AddParameter("body", notificationMessage);

        var response = await client.ExecuteAsync(request);

        return (response?.StatusCode == HttpStatusCode.OK &&
                !string.IsNullOrWhiteSpace(response?.Content));
    }
}