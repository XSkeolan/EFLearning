using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<Message?> GetLastMessage(Guid destinationId);
        Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId);
    }
}