using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;
using MessengerLibrary;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Services
{
    public class ChatService : IChatService
    {
        private readonly IServiceContext _serviceContext;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IChatLinkRepository _chatLinkRepository;

        public ChatService(IServiceContext serviceContext,
            IUserChatRepository userChatRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IFileRepository fileRepository,
            IChatLinkRepository chatLinkRepository)
        {
            _serviceContext = serviceContext;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
            _fileRepository = fileRepository;
            _chatLinkRepository = chatLinkRepository;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            chat.CreatorId = _serviceContext.UserId;
            await _chatRepository.CreateAsync(chat);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            if (!await CurrentUserHaveRights(chatId, Permissions.DELETE_CHAT))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.DeleteAsync(await GetChatAsync(chatId));

            IEnumerable<UserChat> users = await _userChatRepository.GetChatUsersAsync(chatId);
            foreach (UserChat user in users)
            {
                await _userChatRepository.DeleteAsync(user);
            }
        }

        public async Task<Chat> GetChatAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.FindByIdAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            UserChat? userchat = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            UserType? userType = await _userTypeRepository.GetByRoleNameAsync("subscriber") 
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);

            if(userchat == null && chat.DefaultUserTypeId != userType.Id)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            return chat;
        }

        public int GetCountUsers(Guid chatId)
        {
            return _userChatRepository.GetCountUsersInChat(chatId);
        }

        public async Task<UserChat> InviteUserAsync(Guid chatId, Guid userId)
        {
            Chat chat = await GetChatAsync(chatId);

            User? user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            int countUsers = _userChatRepository.GetCountUsersInChat(chatId);

            if (countUsers > 0 && !await CurrentUserHaveRights(chatId, Permissions.INVITE_USER))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
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

        public async Task KickUserAsync(Guid chatId, Guid userId)
        {
            if (!await CurrentUserHaveRights(chatId, Permissions.KICK_USER))
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

        public async Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId)
        {
            Chat chat = await GetChatAsync(chatId);
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

            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_PERMISSION))
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

        public async Task EditNameAsync(Guid chatId, string name)
        {
            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            Chat chat = await GetChatAsync(chatId);
            chat.Name = name;
            await _chatRepository.UpdateAsync(chat);
        }

        public async Task EditDescriptionAsync(Guid chatId, string newDescription)
        {
            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            Chat chat = await GetChatAsync(chatId);
            chat.Description = newDescription;
            await _chatRepository.UpdateAsync(chat);
        }

        public async Task ChangeCreatorAsync(Guid chatlId, Guid userId)
        {
            if (!await CurrentUserHaveRights(chatlId, Permissions.CHANGE_CREATOR))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            if (await _userRepository.FindByIdAsync(userId) == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            UserChat? currentAdmin = await _userChatRepository.GetByChatAndUserAsync(chatlId, _serviceContext.UserId);
            if (currentAdmin == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserChat? newAdmin = await _userChatRepository.GetByChatAndUserAsync(chatlId, userId);
            if (newAdmin == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? moderatorTypeId = await _userTypeRepository.GetByRoleNameAsync("moderator");
            if (moderatorTypeId == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }

            Chat chat = await GetChatAsync(chatlId);
            chat.CreatorId = userId;
            await _chatRepository.UpdateAsync(chat);

            newAdmin.UserTypeId = currentAdmin.UserTypeId;
            await _userChatRepository.UpdateAsync(newAdmin);

            currentAdmin.UserTypeId = moderatorTypeId.Id;
            await _userChatRepository.UpdateAsync(currentAdmin);
        }

        public async Task<IEnumerable<Chat>> GetDialogsAsync(Guid? offset_id, int count)
        {
            IEnumerable<Guid> chats = (await _userChatRepository.GetUserChatsAsync(_serviceContext.UserId)).Select(uc => uc.ChatId);
            if (offset_id.HasValue)
            {
                chats = chats.SkipWhile(x => x != offset_id).Skip(1);
            }

            chats = chats.Take(count);
            if (!chats.Any())
            {
                throw new ArgumentException(ResponseErrors.USER_LIST_CHATS_IS_EMPTY);
            }
            return await Task.WhenAll(chats.Select(async x => await _chatRepository.FindByIdAsync(x) ?? throw new InvalidOperationException(ResponseErrors.CHAT_NOT_FOUND)));
        }

        public async Task<IEnumerable<UserType>> GetRolesAsync()
        {
            return await _userTypeRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(Guid chatId, string nickname)
        {
            await GetChatAsync(chatId);

            IEnumerable<Guid> users = (await _userChatRepository.GetChatUsersAsync(chatId)).Select(uc => uc.UserId);
            List<User> results = new List<User>();
            foreach (Guid user in users)
            {
                User? foundUser = await _userRepository.FindByIdAsync(user);
                if (foundUser != null && foundUser.Nickname.Contains(nickname))
                {
                    results.Add(foundUser);
                }
            }

            return results;
        }

        public async Task EditPhotoAsync(Guid chatId, Guid fileId)
        {
            Chat chat = await GetChatAsync(chatId);
            MessengerDAL.Models.File? file = await _fileRepository.FindByIdAsync(fileId);
            if (file == null)
            {
                throw new ArgumentException(ResponseErrors.FILE_NOT_FOUND);
            }

            chat.PhotoId = file.Id;
            await _chatRepository.UpdateAsync(chat);
        }

        public async Task<UserChat> JoinAsync(Guid chatId)
        {
            Chat chat = await GetChatAsync(chatId);
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

        public async Task LeaveAsync(Guid chatId)
        {
            await GetChatAsync(chatId);

            UserChat? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId)
                ?? throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);

            await _userChatRepository.DeleteAsync(userGroup);
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

        private async Task<bool> CurrentUserHaveRights(Guid chatId, string right, Guid? userId = null)
        {
            Chat chat = await GetChatAsync(chatId);

            UserChat? currentUserChat = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUserChat == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? currentUserRole = await _userTypeRepository.FindByIdAsync(currentUserChat.UserTypeId);
            if (currentUserRole == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }
            if (currentUserRole.Permissions == null)
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

                UserType? userType = await _userTypeRepository.FindByIdAsync(userGroup.UserTypeId);
                if (userType == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
                }
                return currentUserRole.Permissions.Contains(right) && currentUserRole.PriorityLevel <= userType.PriorityLevel;
            }
            else
            {
                return _userChatRepository.GetCountUsersInChat(chatId) == 1 || currentUserRole.Permissions.Contains(right);
            }
        }
    }
}
