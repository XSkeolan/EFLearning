using Microsoft.AspNetCore.Mvc;
using Messenger.DTOs;
using System.Text.RegularExpressions;
using MessengerDAL.Models;
using Messenger.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;

namespace Messenger.Controllers
{
    [ApiController]
    [Route("api/private/auth")]
    public class AuthentificationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILinkService _linkService;

        public AuthentificationController(IUserService userService, ITokenService tokenService, ILinkService linkService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _linkService = linkService;
        }

        [HttpPost("/api/public/auth/signUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            if (!Regex.IsMatch(request.Phonenumber, @"\d{11}") || request.Phonenumber.Length != 11)
            {
                return BadRequest(ResponseErrors.INVALID_PHONE);
            }

            if (request.Name.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (request.Surname.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (request.Password.Length > 32 || request.Password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }
            if (request.Nickname.Length > 20)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            User user = new User
            {
                Phone = request.Phonenumber,
                Name = request.Name,
                Surname = request.Surname,
                Nickname = request.Nickname,
                Password = request.Password
            };

            try
            {
                await _userService.CreateUserAsync(user);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/public/auth/signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            if (!Regex.IsMatch(request.Phonenumber, @"\d{11}") || request.Phonenumber.Length != 11)
            {
                return BadRequest(ResponseErrors.INVALID_PHONE);
            }

            if (request.Password.Length > 32 || request.Password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            try
            {
                Session session = await _userService.SignInAsync(request.Phonenumber, request.Password, Request.Headers.UserAgent);

                return Ok(new SignInResponse
                {
                    UserId = session.UserId,
                    Expiries = _userService.SessionExpires,
                    Token = await _tokenService.CreateSessionToken(session.Id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpHead("signOut")]
        [Authorize]
        public async new Task<IActionResult> SignOut()
        {
            await _userService.SignOutAsync();
            return Ok();
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                User? user = await _userService.GetUserAsync(id);

                return Ok(new UserFullResponse
                {
                    Id = id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname,
                    Status = user.Status ?? "",
                    Email = user.Email ?? "",
                    IsConfirmed = user.IsConfirmed,
                    Phonenumber = user.Phone
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) &&
                string.IsNullOrWhiteSpace(request.Surname) &&
                string.IsNullOrWhiteSpace(request.NickName))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            if (request.Name.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (request.Surname.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (request.NickName.Length > 20)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    MailAddress m = new MailAddress(request.Email);
                }
                catch (FormatException)
                {
                    return BadRequest(ResponseErrors.INVALID_FIELDS);
                }
            }

            try
            {
                await _userService.UpdateUserInfoAsync(request.Name, request.Surname, request.NickName, request.Email);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("updateStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromBody] string newStatus)
        {
            if (newStatus.Length > 255)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            try
            {
                await _userService.UpdateStatusAsync(newStatus);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpDelete("user")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody] string reason)
        {
            if (reason.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            try
            {
                await _userService.DeleteUserAsync(reason);
                await _userService.SignOutAsync();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok();
        }

        [HttpPatch("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            if (newPassword.Length > 32 || newPassword.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            try
            {
                await _userService.ChangePasswordAsync(null, newPassword);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("sendLinkOnEmail")]
        [Authorize]
        public async Task<IActionResult> SendLink()
        {
            string link;
            try
            {
                string emailToken = await _tokenService.CreateEmailToken();
                link = await _linkService.GetEmailLink(emailToken);
                User? user = await _userService.GetCurrentUserAsync();
                if(user == null)
                {
                    return BadRequest(ResponseErrors.USER_NOT_AUTHENTIFICATION);
                }
                if(string.IsNullOrWhiteSpace(user.Email))
                {
                    return BadRequest(ResponseErrors.USER_EMAIL_NOT_SET);
                }
                await _userService.SendToEmailAsync(user.Email, "Подтверждение почты", "Мы рады, что вы используете наш сервис. Чтобы подтвердить ваш аккаунт, перейдите по ссылке\n" + link);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/api/public/auth/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string emailToken)
        {
            if (string.IsNullOrWhiteSpace(emailToken))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            return Ok(await _userService.ConfirmEmailAsync(emailToken));
        }

        [HttpPost("/api/public/auth/recover")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(string code, string newPassword)
        {
            if (code.Length < 6)
            {
                return BadRequest(ResponseErrors.INVALID_CODE);
            }
            if (newPassword.Length > 32 || newPassword.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            try
            {
                ConfirmationCode codeInfo = await _userService.TryGetCodeInfoAsync(code);
                await _userService.ChangePasswordAsync(codeInfo.UserId, newPassword);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("/api/public/auth/requestCode")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestCode([FromBody] string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                await _userService.SendCodeAsync(email);
                return Ok();
            }
            catch (FormatException)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
