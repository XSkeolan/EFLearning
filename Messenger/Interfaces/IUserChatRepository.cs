using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IUserChatRepository : IRepository<UserChat>
    {
        Task<UserChat?> GetByChatAndUserAsync(Guid chatId, Guid userId);
        Task<IEnumerable<UserChat>> GetChatUsersAsync(Guid chatId);
        int GetCountUsersInChat(Guid chatId);
        Task<IEnumerable<UserChat>> GetUserChatsAsync(Guid chatId);
    }
}