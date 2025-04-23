using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationWebApi.Business;
using NotificationWebApi.Business.SMSs;
using NotificationWebApi.Requests;
using NotificationWebApi.Responses;

namespace NotificationWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SMSController : ControllerBase
{
    private readonly ISMSNotification _smsNotification;
    private readonly IValidator<SendMessageRequest> _validator;
    private readonly IAppointmentMessageService _appointmentMessageService;

    public SMSController(
        ISMSNotification smsNotification,
        IValidator<SendMessageRequest> validator,
        IAppointmentMessageService appointmentMessageService)
    {
        _smsNotification = smsNotification ?? throw new ArgumentNullException(nameof(smsNotification));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _appointmentMessageService = appointmentMessageService ?? throw new ArgumentNullException(nameof(appointmentMessageService));
    }

    /// <summary>
    /// Send an SMS notification
    /// </summary>
    /// <returns>true if the message is sent, or false otherwise</returns>
    /// <response code="200">With true or false</response>
    /// <response code="400">For validation errors</response>
    /// <response code="500">For internal server errors</response>
    [HttpPost("send")]
    [ProducesResponseType(200, Type = typeof(SendMessageResponse))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return BadRequest(result);
        }

        try
        {
            var message = _appointmentMessageService.GenerateMessage(
                request.PatientName,
                request.LanguageCode,
                request.DateTime);

            var response = await _smsNotification.SendSMS(request.PhoneNumber, message);

            var sendMessageResponse = new SendMessageResponse { IsMessageSent = response };

            return Ok(sendMessageResponse);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    }
}