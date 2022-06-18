using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IServiceContext
    {
        Guid SessionId { get; }
        Guid UserId { get; }

        void ConfigureSession(Session session);
    }
}