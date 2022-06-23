using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByPhoneAsync(string phone);
        Task<User?> GetConfirmedUserAsync(string email);
        Task<bool> UserIsUniqueAsync(User user);
    }
}
