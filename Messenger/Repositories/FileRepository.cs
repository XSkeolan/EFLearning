using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Repositories
{
    public class FileRepository : BaseRepository<MessengerDAL.Models.File>, IFileRepository
    {
        public FileRepository(MessengerContext context) : base(context)
        {
        }
    }
}
