using Messenger.Interfaces;
using MessengerDAL.Models;

namespace Messenger.Contexts
{
    public class ServiceContext : IServiceContext
    {
        private Guid _userId;
        private Guid _sessionId;

        public Guid UserId { get => _userId; }
        public Guid SessionId { get => _sessionId; }

        public void ConfigureSession(Session session)
        {
            _sessionId = session.Id;
            _userId = session.UserId;
        }
    }
}
