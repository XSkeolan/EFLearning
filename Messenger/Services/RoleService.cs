using Messenger.Interfaces;
using Messenger.Repositories;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;
        private readonly IPermissionService _permissionService;
        private readonly IChatRepository _chatRepository;

        public RoleService(IUserChatRepository userChatRepository,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IServiceContext serviceContext,
            IPermissionService permissionService,
            IChatRepository chatRepository)
        {
            _userChatRepository = userChatRepository;
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
            _serviceContext = serviceContext;
            _permissionService = permissionService;
            _chatRepository = chatRepository;
        }

        public async Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId)
        {
            Chat chat = await _chatRepository.FindByIdAsync(chatId)
                ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            _ = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            User? user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            UserChat? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
            if (userGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? userType = await _userTypeRepository.FindByIdAsync(userGroup.UserTypeId);
            UserChat? currentUser = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUser == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? currnetUserType = await _userTypeRepository.FindByIdAsync(currentUser.UserTypeId);
            if (currnetUserType == null || userType == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }

            if (currnetUserType.PriorityLevel <= userType.PriorityLevel && chat.CreatorId != userId)
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            UserType? newRole = await _userTypeRepository.FindByIdAsync(roleId);
            if (newRole == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_ROLE_NOT_FOUND);
            }

            int countUsers = _userChatRepository.GetCountUsersInChat(chatId);

            if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.EDIT_PERMISSION))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            userGroup.UserTypeId = newRole.Id;
            await _userChatRepository.UpdateAsync(userGroup);
        }

        public async Task<UserType> GetAdminRoleAsync()
        {
            return await _userTypeRepository.GetByRoleNameAsync("admin")
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
        }

        public async Task<IEnumerable<UserType>> GetRolesAsync()
        {
            return await _userTypeRepository.GetAllAsync();
        }
    }
}
