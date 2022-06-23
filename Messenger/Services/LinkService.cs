using Messenger.Interfaces;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class LinkService : ILinkService
    {
        private readonly IServiceContext _serviceContext;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IChatLinkRepository _chatLinkRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypeRepository;

        public LinkService(IServiceContext serviceContext,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            IChatLinkRepository chatLinkRepository,
            IUserChatRepository userChatRepository,
            IUserTypeRepository userTypeRepository)
        {
            _serviceContext = serviceContext;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _chatLinkRepository = chatLinkRepository;
            _userChatRepository = userChatRepository;
            _userTypeRepository = userTypeRepository;
        }

        public async Task<string> GetEmailLink(string emailToken)
        {
            User? user = await _userRepository.FindByIdAsync(_serviceContext.UserId);
            if(user == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException(ResponseErrors.USER_EMAIL_NOT_SET);
            }

            return "api/auth/confirm" + "?e=" + emailToken;
        }

        public async Task CreateInvitationLinkAsync(ChatLink channelLink)
        {
            Chat? chat = await _chatRepository.FindByIdAsync(channelLink.ChatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            if (!await CheckPermission(chat.Id, Permissions.INVITE_LINK))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatLinkRepository.CreateAsync(channelLink);
        }

        public async Task DeleteInvitationLinkAsync(Guid channelLinkId)
        {
            ChatLink? link = await _chatLinkRepository.FindByIdAsync(channelLinkId);
            if (link == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_NOT_FOUND);
            }

            if (!await CheckPermission(link.ChatId, Permissions.INVITE_LINK))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatLinkRepository.DeleteAsync(link);
        }

        private async Task<bool> CheckPermission(Guid chatId, string permission)
        {
            UserChat? userChat = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if(userChat == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? userType = await _userTypeRepository.FindByIdAsync(userChat.UserTypeId);
            if(userType == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }

            return userType.Permissions.Contains(permission);
        }
    }
}
