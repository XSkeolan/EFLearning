using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IUserTypeRepository : IRepository<UserType>
    {
        Task<IEnumerable<UserType>> GetAllAsync();
        Task<UserType?> GetByRoleName(string roleName);
    }
}