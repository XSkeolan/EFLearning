using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(MessengerContext context) : base(context)
        {
        }

        public async Task<bool> UserIsUniqueAsync(User user)
        {
            return await _dbSet.AnyAsync(u => !u.IsDeleted && (u.Phone == user.Phone || u.Nickname == user.Nickname || (u.Email == user.Email && u.IsConfirmed)));
        }

        public async Task<User?> FindByPhoneAsync(string phone)
        {
            return (await GetByConditions(u => u.Phone == phone)).FirstOrDefault();
        }

        public async Task<User?> GetConfirmedUserAsync(string email)
        {
            return await FirstOrDefaultAsync(u => u.IsConfirmed && u.Email == email && !u.IsDeleted);
        }
    }
}
