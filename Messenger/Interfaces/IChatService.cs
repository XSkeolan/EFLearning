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
        Task<Chat> TryGetChatAsync(Guid chatId);

        int GetCountUsers(Guid chatId);
        Task<IEnumerable<Chat>> GetDialogsAsync(Guid? offset_id, int count);
        Task<IEnumerable<User>> SearchUsersAsync(Guid chatId, string nickname);
    }
}