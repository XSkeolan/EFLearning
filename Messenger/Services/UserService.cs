using MessengerDAL.Models;
using MessengerDAL;
using Messenger.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Messenger.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MessengerLibrary;

namespace Messenger.Services
{
    public class UserService : IUserService
    {
        private readonly MessengerContext _messengerContext;
        private readonly IServiceContext _serviceContext;

        private readonly int _sessionExpires;
        private readonly int _emailLinkExpires;

        private readonly string _email;
        private readonly string _password;
        private readonly string _name;
        private readonly string _server;
        private readonly int _port;

        private readonly TimeSpan _validTime;

        public UserService(IOptions<EmailOptions> emailOptions, IOptions<JwtOptions> options, IOptions<CodeOptions> codeOptions, MessengerContext messengerContext, IServiceContext serviceContext)
        {
            _messengerContext = messengerContext;
            _serviceContext = serviceContext;

            _sessionExpires = options.Value.SessionExpires;
            _emailLinkExpires = options.Value.EmailLinkExpires;

            _email = emailOptions.Value.Email;
            _password = emailOptions.Value.Password;
            _name = emailOptions.Value.Name;
            _server = emailOptions.Value.SmtpServer;
            _port = emailOptions.Value.Port;

            _validTime = TimeSpan.FromSeconds(codeOptions.Value.ValidCodeTime);
        }

        public int SessionExpires => _sessionExpires;

        public int EmailLinkExpires => _emailLinkExpires;

        public async Task CreateUserAsync(User user)
        {
            if (_messengerContext.Users.Any(u => u.Phone == user.Phone || u.Nickname == user.Nickname || (u.Email == user.Email && u.IsConfirmed) && !u.IsDeleted))
                throw new ArgumentException(ResponseErrors.USER_ALREADY_EXIST);

            if (!string.IsNullOrWhiteSpace(user.Email) && await _messengerContext.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new ArgumentException(ResponseErrors.EMAIL_ALREADY_EXIST);
            }

            user.Password = Password.GetHashedPassword(user.Password);

            await _messengerContext.AddAsync(user);
            await _messengerContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string reason)
        {
            User? user = await _messengerContext.Users.FindAsync(_serviceContext.UserId);
            if(user == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_FOUND);
            }
            user.Reason = reason;
            user.IsDeleted = true;
            await _messengerContext.SaveChangesAsync();
        }

        public async Task<Session> SignInAsync(string phone, string password, string device)
        {
            User? user = await _messengerContext.Users.FirstOrDefaultAsync(u => u.Phone == phone);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            Password.VerifyHashedPassword(user.Password, password);
            
            if (_messengerContext.Sessions.Any(s => s.DeviceName == device && s.DateEnd >= DateTime.UtcNow))
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_AUTHORIZE);
            }

            Session session = new Session
            {
                DateStart = DateTime.UtcNow,
                UserId = user.Id,
                DeviceName = device,
                DateEnd = DateTime.UtcNow.AddSeconds(_sessionExpires)
            };

            await _messengerContext.AddAsync(session);
            await _messengerContext.SaveChangesAsync();

            return session;
        }

        public async Task SignOutAsync()
        {
            Session? session = await _messengerContext.Sessions.FindAsync(_serviceContext.SessionId);
            if(session == null)
            {
                throw new InvalidOperationException(ResponseErrors.SESSION_NOT_FOUND);
            }
            if(session.DateEnd < DateTime.UtcNow)
            {
                throw new InvalidOperationException(ResponseErrors.SESSION_ALREADY_ENDED);
            }
            session.DateEnd = DateTime.UtcNow;
            await _messengerContext.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            User? user = await _messengerContext.Users.FindAsync(id);
            if (id != _serviceContext.UserId && user != null)
            {
                User? currentUser = await _messengerContext.Users.FindAsync(_serviceContext.UserId);
                if (currentUser == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
                }

                IEnumerable<Guid> firstUserChats = user.Chats.Select(chat => chat.Id);
                IEnumerable<Guid> secondUserChats = currentUser.Chats.Select(chat => chat.Id);

                if (!firstUserChats.Intersect(secondUserChats).Any())
                {
                    throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
                }
            }

            return user ?? throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
        }

        ///Можно переделать
        public async Task<bool> ConfirmEmailAsync(string emailToken)
        {
            if (string.IsNullOrWhiteSpace(emailToken))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            JwtSecurityToken token;
            try
            {
                token = new JwtSecurityTokenHandler().ReadJwtToken(emailToken);
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_TOKEN);
            }

            Claim? userIdTokenClaim = token.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType && claim.ValueType == "Guid");
            Claim? emailTokenClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Email");

            if (userIdTokenClaim == null || emailTokenClaim == null || !Guid.TryParse(userIdTokenClaim.Value, out Guid userId))
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_TOKEN);
            }

            User? user = await _messengerContext.Users.FindAsync(userId);
            if (user == null || user.Email != emailTokenClaim.Value)
            {
                return false;
            }
            if (!user.IsConfirmed)
            {
                user.IsConfirmed = true;
                await _messengerContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task UpdateUserInfoAsync(string name, string surname, string nickname, string email)
        {
            User user = await _messengerContext.Users.FindAsync(_serviceContext.UserId) ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            user.Name = name;
            user.Surname = surname;
            user.Nickname = nickname;

            if (email != user.Email)
            {
                user.Email = email;
                user.IsConfirmed = false;
            }

            await _messengerContext.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(string newStatus)
        {
            User user = await _messengerContext.Users.FindAsync(_serviceContext.UserId) ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            user.Status = newStatus;

            await _messengerContext.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(Guid? userid, string newPassword)
        {
            User user;
            if (userid.HasValue)
            {
                user = await _messengerContext.Users.FindAsync(userid.Value) ?? throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            else
            {
                user  = await _messengerContext.Users.FindAsync(_serviceContext.UserId) ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            }

            string hasedPassword = Password.GetHashedPassword(newPassword);
            if (user.Password == hasedPassword)
            {
                throw new ArgumentException(ResponseErrors.PASSWORD_ALREADY_SET);
            }
            user.Password = newPassword;
            await _messengerContext.SaveChangesAsync();
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            return await _messengerContext.Users.FindAsync(_serviceContext.UserId);
        }

        public async Task SendToEmailAsync(string email, string subject, string content)
        {
            MailAddress from = new MailAddress(_email, _name);
            MailAddress to = new MailAddress(email);

            MailMessage m = new MailMessage(from, to)
            {
                Subject = subject,
                Body = content
            };

            SmtpClient smtp = new SmtpClient(_server, _port)
            {
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(m);
        }

        public async Task<ConfirmationCode> TryGetCodeInfoAsync(string code)
        {
            IEnumerable<ConfirmationCode> confirmationCodes = 
                _messengerContext.ConfirmationCodes.Where(code => code.IsUsed == false && code.DateStart >= DateTime.UtcNow.Add(-_validTime) && !code.IsDeleted);

            if (!confirmationCodes.Any())
            {
                throw new ArgumentException(ResponseErrors.INVALID_CODE);
            }

            foreach (ConfirmationCode codeHash in confirmationCodes)
            {
                try
                {
                    Password.VerifyHashedPassword(codeHash.Code, code);
                    codeHash.IsUsed = true;
                    await _messengerContext.SaveChangesAsync();

                    return codeHash;
                }
                catch (UnauthorizedAccessException) { }
            }

            throw new ArgumentException(ResponseErrors.INVALID_CODE);
        }

        public async Task SendCodeAsync(string email)
        {
            User user = await _messengerContext.Users.Where(u => u.IsConfirmed && u.Email == email && !u.IsDeleted).FirstOrDefaultAsync() 
                ?? throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);

            CodeGenerator codeGenerator = new CodeGenerator(_messengerContext);
            codeGenerator.SetPreviousCodeAsInvalid(user.Id);
            ConfirmationCode code = await codeGenerator.GenerateForUser(user.Id);

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SendToEmailAsync(user.Email, "Код восстановления", generatedCode);
            }
        }
    }
}
