using NotificationWebApi.Business.SMSs;
using NotificationWebApi.Business.WhatApps;

namespace NotificationWebApi.Modules.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<ISMSNotification, SMSNotificationWithAzure>();
        services.AddTransient<IWhatAppNotification, WhatAppNotificationWithUltramsg>();

        return services;
    }
}