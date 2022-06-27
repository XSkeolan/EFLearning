using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface ILinkService
    {
        Task<string> GetEmailLink(string emailToken);
        Task CreateInvitationLinkAsync(ChatLink channelLink);
        Task DeleteInvitationLinkAsync(Guid channelLinkId);
    }
}