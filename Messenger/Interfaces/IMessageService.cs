using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IMessageService
    {
        Task SendMessageAsync(Message message);
        Task<Message> GetMessageAsync(Guid messageId);
        Task DeleteMessageAsync(Guid messagesId);
        Task EditMessageAsync(Guid messageId, string newText);
        Task ChangePinStatusAsync(Guid messageId, bool status);
        Task ReadMessageAsync(Guid messageId);
    }
}