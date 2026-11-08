using RAGAPI.Models;
using RAGAPI.Services.Interfaces;
using CommunityToolkit.VectorData.InMemory;
using CommunityToolkit.VectorData.Qdrant;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Qdrant.Client;
using System.Numerics.Tensors;

namespace RAGAPI.Services.Implementations
{
    public class ChatService : IChatService
    {

        private readonly IChatClient _chatClient;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator; 
        private readonly IChatHistoryService _chatHistoryService; 
        private QdrantCollection<Guid, DocumentChunk> _collection;
        private readonly QdrantClient _qdrantClient;

        public ChatService(IChatClient chatClient, IChatHistoryService chatHistoryService,
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, QdrantClient qdrantClient)
        {
                _chatClient = chatClient;
                _chatHistoryService = chatHistoryService;
                _embeddingGenerator = embeddingGenerator; 
                _qdrantClient = qdrantClient;
        }

        public string StartConversation()
        {
            var conversationId = Guid.NewGuid().ToString();
            _chatHistoryService.AddNewChatHistory(conversationId); 
            return conversationId; 
        }

        public async Task<string> SendMessageAsync(string conversationId, string message, CancellationToken cancellationToken)
        {
            var chatHistory = _chatHistoryService.GetChatHistory(conversationId);
            if(chatHistory == null)
                throw new KeyNotFoundException($"No chat history found for conversation ID: {conversationId}");

            chatHistory.Messages.Add(new ChatMessage(ChatRole.User, message));
           var userEmbedding =  await GenerateEmbedding(message);

            _collection = new QdrantCollection<Guid, DocumentChunk>(
                               _qdrantClient,
                               "documentChunks",
                               ownsClient: true);

            //var options = new VectorSearchOptions<DocumentChunk>
            //{

            //    ScoreThreshold = 0.5,

            //};

            IAsyncEnumerable<VectorSearchResult<DocumentChunk>> searchResults =
                _collection.SearchAsync(userEmbedding, top: 5);

            await foreach (VectorSearchResult<DocumentChunk> result in searchResults)
            {
                 Console.WriteLine($"Found: {result.Record.Content} (score: {result.Score:F4})");
            }
          
            var res = await searchResults.Select(x => x.Record.Content).ToListAsync();
            var context = string.Join(
                "\n\n---\n\n",
                res);

            var prompt = $"""
            Answer the user's question using the provided context.

            Context:
            {context}

            User question:
            {message}

            If the answer is present in the context, answer using that information.
            """;

            var response = await _chatClient.GetResponseAsync(prompt, null, cancellationToken);
            _chatHistoryService.UpdateChatHistory(conversationId, response);
            return response?.Messages.Where(m=> m.Role == ChatRole.Assistant)?.LastOrDefault().Text ?? "";
        }

        private async Task<ReadOnlyMemory<float>> GenerateEmbedding(string message)
        {
           var embedding =  await _embeddingGenerator.GenerateAsync(message); 
           return embedding.Vector;
        }
    
    }
}
