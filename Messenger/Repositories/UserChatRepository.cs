using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Repositories
{
    public class UserChatRepository : BaseRepository<UserChat>, IUserChatRepository
    {
        public UserChatRepository(MessengerContext context) : base(context)
        {
        }

        public async Task<UserChat?> GetByChatAndUserAsync(Guid chatId, Guid userId)
        {
            return (await GetByConditions(userchat => userchat.ChatId == chatId && userchat.UserId == userId && !userchat.IsDeleted)).FirstOrDefault();
        }

        public async Task<IEnumerable<UserChat>> GetChatUsersAsync(Guid chatId)
        {
            return await GetByConditions(uc => uc.ChatId == chatId && !uc.IsDeleted);
        }

        public async Task<IEnumerable<UserChat>> GetUserChatsAsync(Guid userId)
        {
            return await GetByConditions(uc => uc.UserId == userId && !uc.IsDeleted);
        }

        public int GetCountUsersInChat(Guid chatId)
        {
            return _dbSet.Count(u => u.ChatId == chatId);
        }     
    }
}
