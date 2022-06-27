namespace Messenger.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateSessionTokenAsync(Guid sessionId);
        Task<string> CreateEmailTokenAsync();
        Task<string> CreateInvitationTokenAsync(Guid channelLinkId);
    }
}