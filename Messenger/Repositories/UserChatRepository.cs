using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Repositories
{
    public class UserChatRepository : IUserChatRepository
    {
        private readonly MessengerContext _messengerContext;

        public UserChatRepository(MessengerContext messengerContext)
        {
            _messengerContext = messengerContext;
        }

        public async Task<UserChat?> GetByChatAndUserAsync(Guid chatId, Guid userId)
        {
            return await _messengerContext.UserChats.Where(userchat => userchat.ChatId == chatId && userchat.UserId == userId && !userchat.IsDeleted).FirstOrDefaultAsync();
        }

        public Task<IEnumerable<Guid>> GetChatUsersAsync(Guid chatId)
        {
            return Task.Run(() => _messengerContext.UserChats.Where(userchat => userchat.ChatId == chatId).Select(uc => uc.UserId).AsEnumerable());
        }
    }
}
