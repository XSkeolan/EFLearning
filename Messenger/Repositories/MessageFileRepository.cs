using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Repositories
{
    public class MessageFileRepository : BaseRepository<MessageFile>, IMessageFileRepository
    {
        public MessageFileRepository(MessengerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MessageFile>> GetMessageFiles(Guid messageId)
        {
            return await GetByConditions(msgf => msgf.MessageId==messageId && !msgf.IsDeleted);
        }
    }
}
