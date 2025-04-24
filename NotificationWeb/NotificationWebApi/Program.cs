using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotificationWebApi.Business.SMSs;
using NotificationWebApi.Business.WhatApps;
using NotificationWebApi.Modules.Jwt;
using NotificationWebApi.Modules.Services;
using NotificationWebApi.Modules.Swagger;
using NotificationWebApi.Requests.Validations;
using System.Text;

namespace NotificationWebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddValidatorsFromAssemblyContaining<SendMessageRequestValidator>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGenWithDocumentationAndSecurity();

        builder.Services.AddServices();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Get the configuration for JWT from the appsettings.json
                var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings));
                var secretKey = jwtSettings["SecretKey"];
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        builder.Services.AddAuthorization();

        builder.Services.Configure<AzureSmsSettings>(
            builder.Configuration.GetSection(nameof(AzureSmsSettings)));

        builder.Services.Configure<UltramsgWhatAppSettings>(
            builder.Configuration.GetSection(nameof(UltramsgWhatAppSettings)));

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}