using RAGAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using RAGAPI.Models;

namespace RAGAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {

        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService; 
        }

        [HttpGet("Start")]
        public IActionResult Start()
        {
            var conversationId = _chatService.StartConversation();
            return Ok(conversationId);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(ChatRequest chatRequest, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _chatService.SendMessageAsync(
                                    chatRequest.ConversationId,
                                    chatRequest.Message,
                                    cancellationToken);

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }



    }
}
