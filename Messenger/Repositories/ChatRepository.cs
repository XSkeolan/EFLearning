using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(MessengerContext context) : base(context)
        {
        }

        //public Guid GetUserChats(Guid chatId)
        //{
        //    re
        //}
    }
}
