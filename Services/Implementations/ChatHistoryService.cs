using RAGAPI.Models;
using RAGAPI.Services.Interfaces;
using Microsoft.Extensions.AI;

namespace RAGAPI.Services.Implementations
{
    public class ChatHistoryService : IChatHistoryService
    {
        private List<ChatHistory> chatHistories = new();
        public  ChatHistory GetChatHistory(string conversationId)
        {
            var chatHistory = chatHistories.FirstOrDefault(ch => ch.ConversationId == conversationId);
            return chatHistory;
        }

        public  void AddNewChatHistory(string conversationId)
        { 
            chatHistories.Add(new ChatHistory { ConversationId = conversationId });
        }

        public void UpdateChatHistory(string conversationId, ChatResponse chatResponse)
        {
            chatHistories.SingleOrDefault(ch => ch.ConversationId == conversationId)?.Messages?.AddMessages(chatResponse);
        }
    }
}
