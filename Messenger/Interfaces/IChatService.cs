using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IChatService
    {
        Task CreateChatAsync(Chat chat);
        Task<UserChat> InviteUserAsync(Guid chatId, Guid userId);
        Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId);
        Task<UserType> GetAdminRoleAsync();
        Task<Chat> GetChatAsync(Guid chatId);
    }
}