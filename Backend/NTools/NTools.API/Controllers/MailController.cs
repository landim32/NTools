using Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NTools.Domain.Interfaces.Services;
using NTools.DTO.Domain;
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

        public MailController(IMailerSendService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("sendMail")]
        public async Task<ActionResult<StatusResult>> Sendmail([FromBody] MailerInfo mail)
        {
            try
            {
                return new StatusResult
                {
                    Sucesso = await _mailService.Sendmail(mail),
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("isValidEmail/{email}")]
        public ActionResult<StatusResult> IsValidEmail(string email)
        {
            try
            {
                return new StatusResult
                {
                    Sucesso = EmailValidator.IsValidEmail(email),
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
