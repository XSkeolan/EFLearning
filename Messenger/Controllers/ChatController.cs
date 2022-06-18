using Messenger.Interfaces;
using Messenger.DTOs;
using MessengerDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Controllers
{
    [Route("api/private/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ITokenService _tokenService;
        private readonly IFileService _fileService;

        public ChatController(IChatService chatService, ITokenService tokenService, IFileService fileService)
        {
            _chatService = chatService;
            _tokenService = tokenService;
            _fileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            Chat chat = new Chat
            {
                Name = request.Name,
                PhotoId = request.PhotoId,
                DateCreated = DateTime.UtcNow,
                DefaultUserTypeId = request.DefaultUserTypeId
            };

            await _chatService.CreateChatAsync(chat);

            await _chatService.InviteUserAsync(chat.Id, chat.CreatorId);
            await _chatService.SetRoleAsync(chat.Id, chat.CreatorId, (await _chatService.GetAdminRoleAsync()).Id);

            ChatResponse response = new ChatResponse
            {
                Id = chat.Id,
                Name = chat.Name,
                CountUsers = 1,
                CreatorId = chat.CreatorId
            };

            return Created($"api/private/chat?id={response.Id}", response);
        }
    }
}
