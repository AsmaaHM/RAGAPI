using Microsoft.Extensions.AI;

namespace RAGAPI.Models
{
    public class ChatHistory
    {
        public string ConversationId { get; set; }

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
