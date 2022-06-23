using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(MessengerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId)
        {
            return await GetByConditions(msg => msg.DestinationChatId == destinationId && !msg.IsDeleted);
        }

        public async Task<Message?> GetLastMessage(Guid destinationId)
        {
            return await _dbSet.Where(msg => msg.DestinationChatId == destinationId).OrderBy(msg => msg.DateSend).FirstOrDefaultAsync();
        }
    }
}
