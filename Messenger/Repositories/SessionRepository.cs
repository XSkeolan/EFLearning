using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(MessengerContext context) : base(context)
        {
        }

        public async Task<Session?> GetUnfinishedOnDeviceAsync(string device, DateTime endTimeSession)
        {
            return await FirstOrDefaultAsync(s => s.DeviceName == device && s.DateEnd >= endTimeSession);
        }
    }
}
