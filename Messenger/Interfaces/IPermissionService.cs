namespace Messenger.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> CurrentUserHaveRight(Guid chatId, string permission, Guid? userId = null);
    }
}