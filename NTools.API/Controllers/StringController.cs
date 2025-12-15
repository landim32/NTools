using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NTools.Domain.Utils;
using System;

namespace BazzucaMedia.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StringController : ControllerBase
    {
        private readonly ILogger<StringController> _logger;

        public StringController(ILogger<StringController> logger)
        {
            _logger = logger;
        }

        [HttpGet("generateSlug/{name}")]
        public ActionResult<string> GenerateSlug(string name)
        {
            try
            {
                var slug = SlugHelper.GenerateSlug(name);
                _logger.LogInformation("Generate Slug '{0}' from string '{1}'", slug, name);
                return Ok(slug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("onlyNumbers/{input}")]
        public ActionResult<string> OnlyNumbers(string input)
        {
            try
            {
                var onlyNumber = StringUtils.OnlyNumbers(input);
                _logger.LogInformation("Extract only numbers `{0}` from {1}", onlyNumber, input);
                return Ok(StringUtils.OnlyNumbers(input));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("generateShortUniqueString")]
        public ActionResult<string> GenerateShortUniqueString()
        {
            try
            {
                var uniqueStr = StringUtils.GenerateShortUniqueString();
                _logger.LogInformation("Generate short unique string: `{0}`", uniqueStr);
                return Ok(StringUtils.GenerateShortUniqueString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
