using RAGAPI.Models;
using Microsoft.Extensions.AI;

namespace RAGAPI.Services.Interfaces
{
    public interface IChatHistoryService
    {
         ChatHistory GetChatHistory(string conversationId);
         void AddNewChatHistory(string conversationId);
         void UpdateChatHistory(string conversationId, ChatResponse chatResponse);
    }
}
