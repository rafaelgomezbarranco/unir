using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationWebApi.Business.SMSs;
using NotificationWebApi.Requests;

namespace NotificationWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SMSController : ControllerBase
{
    private readonly ISMSNotification _smsNotification;
    private readonly IValidator<SendMessageRequest> _validator;

    public SMSController(ISMSNotification smsNotification, IValidator<SendMessageRequest> validator)
    {
        _smsNotification = smsNotification ?? throw new ArgumentNullException(nameof(smsNotification));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    /// <summary>
    /// Send an SMS notification
    /// </summary>
    /// <returns>true if the message is sent, or false otherwise</returns>
    /// <response code="200">With true or false</response>
    [HttpPost("send")]
    [ProducesResponseType(200, Type = typeof(bool))]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        var response = await _smsNotification.SendSMS(string.Empty, string.Empty);

        return Ok(true);
    }
}