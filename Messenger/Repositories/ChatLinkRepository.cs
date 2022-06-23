using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Repositories
{
    public class ChatLinkRepository : BaseRepository<ChatLink>, IChatLinkRepository
    {
        public ChatLinkRepository(MessengerContext context) : base(context)
        {
        }
    }
}
