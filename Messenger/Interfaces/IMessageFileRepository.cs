using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IMessageFileRepository : IRepository<MessageFile>
    {
        Task<IEnumerable<MessageFile>> GetMessageFiles(Guid messageId);
    }
}