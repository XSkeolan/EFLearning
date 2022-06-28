using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IMessageHistoryService
    {
        Task<IEnumerable<Message>> FindMessagesAsync(Guid chatId, string subtext);
        Task<IEnumerable<Message>> GetHistoryAsync(Guid chatId, DateTime dateStart, DateTime dateEnd);
        Task<Message?> GetLastMessageAsync(Guid chatId);
    }
}