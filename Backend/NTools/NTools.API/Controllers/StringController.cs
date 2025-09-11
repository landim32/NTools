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
    public class StringController : ControllerBase
    {
        [HttpGet("generateSlug/{name}")]
        public ActionResult<StringResult> GenerateSlug(string name)
        {
            try
            {
                return new StringResult
                {
                    Sucesso = true,
                    Value = SlugHelper.GenerateSlug(name)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("onlyNumbers/{input}")]
        public ActionResult<StringResult> OnlyNumbers(string input)
        {
            try
            {
                return new StringResult
                {
                    Sucesso = true,
                    Value = StringUtils.OnlyNumbers(input)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("generateShortUniqueString")]
        public ActionResult<StringResult> GenerateShortUniqueString()
        {
            try
            {
                return new StringResult
                {
                    Sucesso = true,
                    Value = StringUtils.GenerateShortUniqueString()
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
