using Messenger.Interfaces;
using MessengerDAL.Models;

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
        private readonly IPermissionService _permissionService;

        public ChatService(IServiceContext serviceContext,
            IUserChatRepository userChatRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IFileRepository fileRepository,
            IChatLinkRepository chatLinkRepository,
            IPermissionService permissionService)
        {
            _serviceContext = serviceContext;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
            _fileRepository = fileRepository;
            _permissionService = permissionService;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            chat.CreatorId = _serviceContext.UserId;
            await _chatRepository.CreateAsync(chat);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.DELETE_CHAT))
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

        public async Task<Chat> TryGetChatAsync(Guid chatId)
        {
            Chat chat = await _chatRepository.FindByIdAsync(chatId)
                ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            UserType? userType = await _userTypeRepository.GetByRoleNameAsync("subscriber")
                ?? throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);

            if (chat.DefaultUserTypeId != userType.Id)
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_PRIVATE);
            }

            return chat;
        }

        private async Task<Chat> GetChatAsync(Guid chatId)
        {
            Chat chat = await _chatRepository.FindByIdAsync(chatId)
                ?? throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            _ = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId)
                ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            return chat;
        }


        public int GetCountUsers(Guid chatId)
        {
            return _userChatRepository.GetCountUsersInChat(chatId);
        }

        public async Task EditNameAsync(Guid chatId, string name)
        {
            if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            Chat chat = await GetChatAsync(chatId);
            chat.Name = name;
            await _chatRepository.UpdateAsync(chat);
        }

        public async Task EditDescriptionAsync(Guid chatId, string newDescription)
        {
            if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            Chat chat = await GetChatAsync(chatId);
            chat.Description = newDescription;
            await _chatRepository.UpdateAsync(chat);
        }

        public async Task ChangeCreatorAsync(Guid chatId, Guid userId)
        {
            if (!await _permissionService.CurrentUserHaveRight(chatId, Permissions.CHANGE_CREATOR))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            if (await _userRepository.FindByIdAsync(userId) == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            UserChat? currentAdmin = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentAdmin == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserChat? newAdmin = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
            if (newAdmin == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? moderatorTypeId = await _userTypeRepository.GetByRoleNameAsync("moderator");
            if (moderatorTypeId == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_TYPE_NOT_FOUND);
            }

            Chat chat = await GetChatAsync(chatId);
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

            List<Chat> chatList = new List<Chat>();
            foreach (Guid chat in chats)
            {
                Chat currentChat = await _chatRepository.FindByIdAsync(chat) ?? throw new InvalidOperationException(ResponseErrors.CHAT_NOT_FOUND);
                chatList.Add(currentChat);
            }
            return chatList;
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
    }
}
