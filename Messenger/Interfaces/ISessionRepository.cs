using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<Session?> GetUnfinishedOnDeviceAsync(string device, DateTime endTimeSession);
    }
}