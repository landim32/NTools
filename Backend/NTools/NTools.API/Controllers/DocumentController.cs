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
    public class DocumentController : ControllerBase
    {
        [HttpGet("validarCpfOuCnpj/{cpfCnpj}")]
        public ActionResult<StatusResult> ValidarCpfOuCnpj(string cpfCnpj)
        {
            try
            {
                return new StatusResult
                {
                    Sucesso = DocumentoUtils.ValidarCpfOuCnpj(cpfCnpj),
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
