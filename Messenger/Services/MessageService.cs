using Messenger.Interfaces;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;

        public MessageService(IMessageRepository messages,
            IChatRepository chatRepository,
            IUserChatRepository userChatRepository,
            IUserTypeRepository userTypeRepository,
            IServiceContext serviceContext)
        {
            _messageRepository = messages;
            _chatRepository = chatRepository;
            _userChatRepository = userChatRepository;
            _userTypeRepository = userTypeRepository;
            _serviceContext = serviceContext;
        }

        public async Task SendMessageAsync(Message message)
        {
            if (message.OriginalMessageId.HasValue)
            {
                if (await _messageRepository.FindByIdAsync(message.OriginalMessageId.Value) == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }
            }
            if (message.ReplyMessageId.HasValue)
            {
                Message? replyMessage = await _messageRepository.FindByIdAsync(message.ReplyMessageId.Value);
                if (replyMessage == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }

                message.DestinationChatId = replyMessage.DestinationChatId;
            }

            if (await _chatRepository.FindByIdAsync(message.DestinationChatId) == null)
            {
                throw new ArgumentException(ResponseErrors.DESTINATION_NOT_FOUND);
            }

            if (await _userChatRepository.GetByChatAndUserAsync(message.DestinationChatId, _serviceContext.UserId) == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            if (string.IsNullOrWhiteSpace(message.Text) && !message.OriginalMessageId.HasValue)
            {
                throw new InvalidOperationException(ResponseErrors.EMPTY_MESSAGE);
            }

            message.FromUserId = _serviceContext.UserId;
            await _messageRepository.CreateAsync(message);
        }

        public async Task<Message> GetMessageAsync(Guid messageId)
        {
            Message message = await _messageRepository.FindByIdAsync(messageId) ?? throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
            _ = await _userChatRepository.GetByChatAndUserAsync(message.DestinationChatId, _serviceContext.UserId)
                ?? throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);

            return message;
        }

        public async Task DeleteMessageAsync(Guid messagesId)
        {
            Message message = await GetMessageAsync(messagesId);
            UserChat userGroup = (await _userChatRepository.GetByChatAndUserAsync(message.DestinationChatId, _serviceContext.UserId))!;
            UserType userType = await _userTypeRepository.FindByIdAsync(userGroup.UserTypeId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);

            if ((userType.Permissions == null || !userType.Permissions.Contains(Permissions.DELETE_MESSAGE)) && message.FromUserId != _serviceContext.UserId)
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _messageRepository.DeleteAsync(message);
        }

        public async Task ChangePinStatusAsync(Guid messageId, bool status)
        {
            Message message = await GetMessageAsync(messageId);
            UserChat userGroup = (await _userChatRepository.GetByChatAndUserAsync(message.DestinationChatId, _serviceContext.UserId))!;
            UserType userType = await _userTypeRepository.FindByIdAsync(userGroup.UserTypeId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);

            if (userType.Permissions == null || !userType.Permissions.Contains(Permissions.PIN_MESSAGE))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }
            message.IsPinned = status;
            await _messageRepository.UpdateAsync(message);
        }

        public async Task EditMessageAsync(Guid messageId, string newText)
        {
            Message message = await GetMessageAsync(messageId);
            if (message.FromUserId != _serviceContext.UserId)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_SENDER);
            }

            message.Text = newText;
            await _messageRepository.UpdateAsync(message);
        }

        public async Task ReadMessageAsync(Guid messageId)
        {
            Message message = await GetMessageAsync(messageId);
            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
        }
    }
}
