using Messenger.Interfaces;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;
        private readonly IUserChatRepository _userChatRepository;

        public PermissionService(IChatRepository chatRepository,
            IUserTypeRepository userTypeRepository,
            IServiceContext serviceContext,
            IUserChatRepository userChatRepository)
        {
            _chatRepository = chatRepository;
            _userTypeRepository = userTypeRepository;
            _serviceContext = serviceContext;
            _userChatRepository = userChatRepository;
        }

        public async Task<bool> CurrentUserHaveRight(Guid chatId, string permission, Guid? userId = null)
        {
            _ = await _chatRepository.FindByIdAsync(chatId) ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            UserChat currentUserChat = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            UserType? currentUserRole = await _userTypeRepository.FindByIdAsync(currentUserChat.UserTypeId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);

            if (userId.HasValue)
            {
                UserChat? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId.Value);
                if (userGroup == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
                }

                UserType? userType = await _userTypeRepository.FindByIdAsync(userGroup.UserTypeId);
                if (userType == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
                }
                if(currentUserRole.Permissions == null)
                {
                    return false;
                }
                return currentUserRole.Permissions.Contains(permission) && currentUserRole.PriorityLevel <= userType.PriorityLevel;
            }
            else
            {
                int countUsers = _userChatRepository.GetCountUsersInChat(chatId);
                if(countUsers == 1)
                {
                    return true;
                }
                if (currentUserRole.Permissions == null)
                {
                    return false;
                }
                return currentUserRole.Permissions.Contains(permission);
            }
        }
    }
}
