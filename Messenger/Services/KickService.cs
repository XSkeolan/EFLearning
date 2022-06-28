using Messenger.Interfaces;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class KickService : IKickService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;
        private readonly IChatLinkRepository _chatLinkRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IPermissionService _permissionService;

        public KickService(
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

        public async Task KickUserAsync(Guid chatId, Guid userId)
        {
            if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.KICK_USER))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            UserChat? kickUserGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
            if (kickUserGroup == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? kickUserType = await _userTypeRepository.FindByIdAsync(kickUserGroup.UserTypeId);
            UserChat? currentUserGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUserGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }
            UserType? currentUserType = await _userTypeRepository.FindByIdAsync(currentUserGroup.UserTypeId);
            if (currentUserType == null || kickUserType == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }
            if (currentUserType.PriorityLevel <= kickUserType.PriorityLevel)
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            if (kickUserType?.Id == currentUserType?.Id)
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_MODER_NOT_DELETED);
            }
            await _userChatRepository.DeleteAsync(kickUserGroup);
        }

        public async Task LeaveAsync(Guid chatId)
        {
            _ = await _chatRepository.FindByIdAsync(chatId) ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);

            UserChat? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId)
                ?? throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);

            await _userChatRepository.DeleteAsync(userGroup);
        }
    }
}
