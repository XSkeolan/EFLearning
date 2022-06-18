namespace Messenger.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateSessionToken(Guid sessionId);
        Task<string> CreateEmailToken();
    }
}