using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IChatService
    {
        Task ChangeCreatorAsync(Guid chatlId, Guid userId);
        Task CreateChatAsync(Chat chat);
        Task DeleteChatAsync(Guid chatId);
        Task EditDescriptionAsync(Guid chatId, string newDescription);
        Task EditNameAsync(Guid chatId, string name);
        Task EditPhotoAsync(Guid chatId, Guid fileId);
        Task<UserType> GetAdminRoleAsync();
        Task<Chat> GetChatAsync(Guid chatId);
        int GetCountUsers(Guid chatId);
        Task<IEnumerable<Chat>> GetDialogsAsync(Guid? offset_id, int count);
        Task<IEnumerable<UserType>> GetRolesAsync();
        Task<UserChat> InviteUserAsync(Guid chatId, Guid userId);
        Task<UserChat> JoinAsync(Guid chatId);
        Task<Chat> JoinByLinkAsync(string token);
        Task KickUserAsync(Guid chatId, Guid userId);
        Task LeaveAsync(Guid chatId);
        Task<IEnumerable<User>> SearchUsersAsync(Guid chatId, string nickname);
        Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId);
    }
}