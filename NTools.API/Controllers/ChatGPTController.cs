using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NTools.Domain.Services.Interfaces;
using NTools.DTO.ChatGPT;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTools.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        private readonly IChatGPTService _chatGPTService;
        private readonly ILogger<ChatGPTController> _logger;

        public ChatGPTController(IChatGPTService chatGPTService, ILogger<ChatGPTController> logger)
        {
            _chatGPTService = chatGPTService;
            _logger = logger;
        }

        [HttpPost("sendMessage")]
        public async Task<ActionResult<string>> SendMessage([FromBody] ChatGPTMessageRequest request)
        {
            try
            {
                _logger.LogInformation("Sending message to ChatGPT");
                var response = await _chatGPTService.SendMessageAsync(request.Message);
                _logger.LogInformation("ChatGPT response received");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to ChatGPT");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("sendConversation")]
        public async Task<ActionResult<string>> SendConversation([FromBody] List<ChatMessage> messages)
        {
            try
            {
                _logger.LogInformation("Sending conversation to ChatGPT with {Count} messages", messages.Count);
                var response = await _chatGPTService.SendConversationAsync(messages);
                _logger.LogInformation("ChatGPT conversation response received");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending conversation to ChatGPT");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("sendRequest")]
        public async Task<ActionResult<ChatGPTResponse>> SendRequest([FromBody] ChatGPTRequest request)
        {
            try
            {
                _logger.LogInformation("Sending custom request to ChatGPT");
                var response = await _chatGPTService.SendRequestAsync(request);
                _logger.LogInformation("ChatGPT custom request response received");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom request to ChatGPT");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("generateImage")]
        public async Task<ActionResult<DallEResponse>> GenerateImage([FromBody] ChatGPTMessageRequest request)
        {
            try
            {
                _logger.LogInformation("Generating image with DALL-E");
                var response = await _chatGPTService.GenerateImageAsync(request.Message);
                _logger.LogInformation("Image generation completed");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating image");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("generateImageAdvanced")]
        public async Task<ActionResult<DallEResponse>> GenerateImageAdvanced([FromBody] DallERequest request)
        {
            try
            {
                _logger.LogInformation("Generating image with DALL-E (advanced)");
                var response = await _chatGPTService.GenerateImageAsync(request);
                _logger.LogInformation("Image generation completed");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating image");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
