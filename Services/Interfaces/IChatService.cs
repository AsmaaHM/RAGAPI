namespace RAGAPI.Services.Interfaces
{
    public interface IChatService
    {
        string StartConversation();
        public Task<string> SendMessageAsync(string converstationId, string message, CancellationToken cancellationToken);
    }
}
