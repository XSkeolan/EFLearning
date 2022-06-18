using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Services
{
    public class ChatService : IChatService
    {
        private readonly MessengerContext _messengerContext;
        private readonly IServiceContext _serviceContext;
        private readonly IUserChatRepository _userChatRepository;

        public ChatService(MessengerContext messengerContext, IServiceContext serviceContext, IUserChatRepository userChatRepository)
        {
            _messengerContext = messengerContext;
            _serviceContext = serviceContext;
            _userChatRepository = userChatRepository;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            chat.CreatorId = _serviceContext.UserId;
            await _messengerContext.Chats.AddAsync(chat);
        }

        public async Task<Chat> GetChatAsync(Guid chatId)
        {
            Chat? chat = await _messengerContext.Chats.Where(chat => chat.Id == chatId)
                .Include(chat => chat.UserChats.Where(us => !us.IsDeleted))
                .FirstOrDefaultAsync();
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            return chat;
        }

        public async Task<UserChat> InviteUserAsync(Guid chatId, Guid userId)
        {
            Chat chat = await GetChatAsync(chatId);

            User? user = await _messengerContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            int countUsers = chat.UserChats.Count;

            if (countUsers > 0 && !await CurrentUserHaveRights(chatId, Permissions.INVITE_USER))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            if ((await _userChatRepository.GetChatUsersAsync(chatId)).Contains(userId))
            {
                throw new ArgumentException(ResponseErrors.USER_ALREADY_IN_CHAT);
            }

            UserType? userType = await _messengerContext.UserTypes.FindAsync(chat.DefaultUserTypeId);
            UserChat userGroup = new UserChat
            {
                ChatId = chatId,
                UserId = userId,
                UserTypeId = userType.Id,
            };

            await _messengerContext.UserChats.AddAsync(userGroup);
            await _messengerContext.SaveChangesAsync();

            return userGroup;
        }

        public async Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId)
        {
            await GetChatAsync(chatId);

            User? user = await _messengerContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            UserChat? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
            if (userGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? userType = await _messengerContext.UserTypes.FindAsync(userGroup.UserTypeId);
            UserChat? currentUser = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUser == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }
            UserType? currnetUserType = await _messengerContext.UserTypes.FindAsync(currentUser.UserTypeId);

            if (currnetUserType.PriorityLevel <= userType.PriorityLevel && (await _userChatRepository.GetUserChatsAsync(chatId)).Any())
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            UserType? newRole = await _messengerContext.UserTypes.FindAsync(roleId);
            if (newRole == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_ROLE_NOT_FOUND);
            }

            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_PERMISSION))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            userGroup.UserTypeId = newRole.Id;
            await _messengerContext.SaveChangesAsync();
        }

        public Task<UserType> GetAdminRoleAsync()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> CurrentUserHaveRights(Guid chatId, string right, Guid? userId = null)
        {
            UserChat? currentUserChat = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUserChat == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? currentUserRole = await _messengerContext.UserTypes.FindAsync(currentUserChat.UserTypeId);
            if (currentUserRole.Permissions == null && (await _userChatRepository.GetChatUsersAsync(chatId)).Count() > 1)
            {
                return false;
            }

            if (userId.HasValue)
            {
                UserChat? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId.Value);
                if (userGroup == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
                }

                UserType? userType = await _messengerContext.UserTypes.FindAsync(userGroup.UserTypeId);
                return currentUserRole.Permissions.Contains(right) && currentUserRole.PriorityLevel <= userType.PriorityLevel;
            }
            else
            {
                return (await _userChatRepository.GetChatUsersAsync(chatId)).Count() == 1 || currentUserRole.Permissions.Contains(right);
            }
        }
    }
}
