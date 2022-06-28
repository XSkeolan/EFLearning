using Messenger.Interfaces;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class MessageHistoryService : IMessageHistoryService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;

        public MessageHistoryService(IMessageRepository messageRepository, IChatRepository chatRepository)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
        }

        public async Task<IEnumerable<Message>> FindMessagesAsync(Guid chatId, string subtext)
        {
            return (await _messageRepository.GetMessagesByDestination(chatId)).Where(x => x.Text.Contains(subtext));
        }

        public async Task<Message?> GetLastMessageAsync(Guid chatId)
        {
            _ = await _chatRepository.FindByIdAsync(chatId) ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            return await _messageRepository.GetLastMessage(chatId);
        }

        public async Task<IEnumerable<Message>> GetHistoryAsync(Guid chatId, DateTime dateStart, DateTime dateEnd)
        {
            Chat? chat = await _chatRepository.FindByIdAsync(chatId) ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);

            IEnumerable<Message> chatMessages = (await _messageRepository.GetMessagesByDestination(chatId))
                .Where(msg => msg.DateSend >= dateStart && msg.DateSend <= dateEnd);

            return chatMessages;
        }
    }
}
