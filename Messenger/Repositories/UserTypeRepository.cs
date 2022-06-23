using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Repositories
{
    public class UserTypeRepository : BaseRepository<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(MessengerContext context) : base(context)
        {
        }

        public async Task<UserType?> GetByRoleName(string roleName)
        {
            return (await GetByConditions(c => c.TypeName == roleName)).FirstOrDefault();
        }

        //Скорее всего поправить
        public async Task<IEnumerable<UserType>> GetAllAsync()
        {
            return await Task.Run(() => _dbSet.ToList());
        }
    }
}
