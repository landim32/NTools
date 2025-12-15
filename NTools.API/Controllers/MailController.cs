using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NTools.Domain.Services.Interfaces;
using NTools.Domain.Utils;
using NTools.DTO.MailerSend;
using System;
using System.Threading.Tasks;

namespace BazzucaMedia.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailerSendService _mailService;
        private readonly ILogger<MailController> _logger;

        public MailController(IMailerSendService mailService, ILogger<MailController> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        [HttpPost("sendMail")]
        public async Task<ActionResult<bool>> Sendmail([FromBody] MailerInfo mail)
        {
            try
            {
                _logger.LogInformation("Sending email to {0}, subject: {1}", mail.To, mail.Subject);
                return Ok(await _mailService.Sendmail(mail));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending mail");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("isValidEmail/{email}")]
        public ActionResult<bool> IsValidEmail(string email)
        {
            try
            {
                _logger.LogInformation("Verify if email {0} is valid", email);
                var isValid = EmailValidator.IsValidEmail(email);
                _logger.LogInformation(isValid ? "Is a valid email" : "Is not a valid email");
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending mail");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
