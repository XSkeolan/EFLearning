namespace Messenger.Interfaces
{
    public interface IKickService
    {
        Task KickUserAsync(Guid chatId, Guid userId);
        Task LeaveAsync(Guid chatId);
    }
}
