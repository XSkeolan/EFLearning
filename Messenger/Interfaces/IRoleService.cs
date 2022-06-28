using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IRoleService
    {
        Task<UserType> GetAdminRoleAsync();
        Task<IEnumerable<UserType>> GetRolesAsync();
        Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId);
    }
}
