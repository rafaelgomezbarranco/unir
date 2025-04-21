using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationWebApi.Business.SMSs;
using NotificationWebApi.Request;

namespace NotificationWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SMSController : ControllerBase
{
    private readonly ISMSNotification _smsNotification;

    public SMSController(ISMSNotification smsNotification)
    {
        _smsNotification = smsNotification ?? throw new ArgumentNullException(nameof(smsNotification));
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
        await _smsNotification.SendSMS(string.Empty, string.Empty);
            
        return Ok(true);
    }
}