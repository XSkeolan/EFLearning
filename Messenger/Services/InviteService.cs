using Messenger.Interfaces;
using MessengerDAL.Models;
using MessengerLibrary;

namespace Messenger.Services
{
    public class InviteService : IInviteService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;
        private readonly IChatLinkRepository _chatLinkRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IPermissionService _permissionService;

        public InviteService(
            IUserRepository userRepository,
            IUserChatRepository userChatRepository,
            IUserTypeRepository userTypeRepository,
            IServiceContext serviceContext,
            IChatLinkRepository chatLinkRepository,
            IChatRepository chatRepository,
            IPermissionService permissionService)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _userTypeRepository = userTypeRepository;
            _serviceContext = serviceContext;
            _chatLinkRepository = chatLinkRepository;
            _chatRepository = chatRepository;
            _permissionService = permissionService;
        }
        public async Task<UserChat> InviteUserAsync(Guid chatId, Guid userId)
        {
            Chat chat = await _chatRepository.FindByIdAsync(chatId)
                ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);

            User? user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            int countUsers = _userChatRepository.GetCountUsersInChat(chatId);

            if (countUsers > 0)
            {
                _ = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId) 
                    ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
                if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.INVITE_USER))
                {
                    throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
                }
            }

            if (await _userChatRepository.GetByChatAndUserAsync(chatId, userId) != null)
            {
                throw new ArgumentException(ResponseErrors.USER_ALREADY_IN_CHAT);
            }

            UserType? userType = await _userTypeRepository.FindByIdAsync(chat.DefaultUserTypeId);
            if (userType == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }

            UserChat userChat = new UserChat
            {
                ChatId = chatId,
                UserId = userId,
                UserTypeId = userType.Id,
            };

            await _userChatRepository.CreateAsync(userChat);

            return userChat;
        }

        public async Task<UserChat> JoinAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.FindByIdAsync(chatId)
                ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            UserType? userType = await _userTypeRepository.GetByRoleNameAsync("subscriber")
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);

            if (userType.Id != chat.DefaultUserTypeId)
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_PRIVATE);
            }

            if ((await _userChatRepository.GetChatUsersAsync(chatId)).Any(uc => uc.UserId == _serviceContext.UserId))
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_IN_CHANNEL);
            }

            UserChat userChannel = new UserChat
            {
                ChatId = chatId,
                UserId = _serviceContext.UserId,
                UserTypeId = chat.DefaultUserTypeId
            };

            await _userChatRepository.CreateAsync(userChannel);

            return userChannel;
        }

        public async Task<Chat> JoinByLinkAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            JwtTokenValidator linkJwt = new JwtTokenValidator();
            ClaimParser claimParser;
            try
            {
                linkJwt.ValidateToken(token);

                claimParser = new ClaimParser(linkJwt.GetAllClaims(), new List<TokenClaimPart> { new GuidTokenClaimPart(), new DateTimeTokenClaimPart() });
                claimParser.ParseTokenClaims();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            DateTime dateEnd = DateTime.Parse(claimParser.TokenParts[1].Value);

            ChatLink? channelLink = await _chatLinkRepository.FindByIdAsync(Guid.Parse(claimParser.TokenParts[0].Value))
                ?? throw new ArgumentException(ResponseErrors.CHANNEL_LINK_ALREADY_USED);

            if (dateEnd < DateTime.UtcNow.ToLocalTime())
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_INVALID);
            }

            if (channelLink.IsOneTime)
            {
                await _chatLinkRepository.DeleteAsync(channelLink);
            }

            Chat? chat = await _chatRepository.FindByIdAsync(channelLink.ChatId) ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            _ = await _userChatRepository.GetByChatAndUserAsync(chat.Id, _serviceContext.UserId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_ALREADY_IN_CHAT);

            UserChat? userGroup = new UserChat
            {
                ChatId = channelLink.ChatId,
                UserId = _serviceContext.UserId,
                UserTypeId = chat.DefaultUserTypeId
            };

            await _userChatRepository.CreateAsync(userGroup);

            return chat;
        }
    }
}
