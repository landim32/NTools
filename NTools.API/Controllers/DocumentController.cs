using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NTools.Domain.Utils;
using System;

namespace BazzucaMedia.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("validarCpfOuCnpj/{cpfCnpj}")]
        public ActionResult<bool> ValidarCpfOuCnpj(string cpfCnpj)
        {
            try
            {
                var isValid = DocumentoUtils.ValidarCpfOuCnpj(cpfCnpj);
                _logger.LogInformation("validarCpfOuCnpj: {@cpfCnpj}={@isValid}", cpfCnpj, isValid);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
