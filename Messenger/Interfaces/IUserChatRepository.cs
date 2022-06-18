using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IUserChatRepository
    {
        Task<UserChat?> GetByChatAndUserAsync(Guid chatId, Guid userId);
        Task<IEnumerable<Guid>> GetChatUsersAsync(Guid chatId);
    }
}