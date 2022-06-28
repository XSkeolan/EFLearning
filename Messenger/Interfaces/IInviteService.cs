using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IInviteService
    {
        Task<UserChat> InviteUserAsync(Guid chatId, Guid userId);
        Task<UserChat> JoinAsync(Guid chatId);
        Task<Chat> JoinByLinkAsync(string token);
    }
}
