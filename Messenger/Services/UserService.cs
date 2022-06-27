using MessengerDAL.Models;
using MessengerDAL;
using Messenger.Interfaces;
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
        private readonly IServiceContext _serviceContext;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IConfirmationCodeRepository _confirmationCodeRepository;
        private readonly int _sessionExpires;
        private readonly int _emailLinkExpires;

        private readonly string _email;
        private readonly string _password;
        private readonly string _name;
        private readonly string _server;
        private readonly int _port;

        private readonly TimeSpan _validTime;

        public UserService(IOptions<EmailOptions> emailOptions,
            IOptions<JwtOptions> options,
            IOptions<CodeOptions> codeOptions,
            IServiceContext serviceContext,
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IConfirmationCodeRepository confirmationCodeRepository)
        {
            _serviceContext = serviceContext;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _confirmationCodeRepository = confirmationCodeRepository;
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
            if (await _userRepository.UserIsUniqueAsync(user))
                throw new ArgumentException(ResponseErrors.USER_ALREADY_EXIST);

            user.Password = Password.GetHashedPassword(user.Password);

            await _userRepository.CreateAsync(user);
        }

        public async Task DeleteUserAsync(string reason)
        {
            User? user = await _userRepository.FindByIdAsync(_serviceContext.UserId);
            if(user == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_FOUND);
            }
            user.Reason = reason;
            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<Session> SignInAsync(string phone, string password, string device)
        {
            User? user = await _userRepository.FindByPhoneAsync(phone);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            Password.VerifyHashedPassword(user.Password, password);
            
            if (await _sessionRepository.GetUnfinishedOnDeviceAsync(device, DateTime.UtcNow) != null)
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

            await _sessionRepository.CreateAsync(session);

            return session;
        }

        public async Task SignOutAsync()
        {
            Session? session = await _sessionRepository.FindByIdAsync(_serviceContext.SessionId);
            if(session == null)
            {
                throw new InvalidOperationException(ResponseErrors.SESSION_NOT_FOUND);
            }
            if(session.DateEnd < DateTime.UtcNow)
            {
                throw new InvalidOperationException(ResponseErrors.SESSION_ALREADY_ENDED);
            }
            session.DateEnd = DateTime.UtcNow;
            await _sessionRepository.UpdateAsync(session);
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            User? user = await _userRepository.FindByIdAsync(id);
            if (id != _serviceContext.UserId && user != null)
            {
                User? currentUser = await _userRepository.FindByIdAsync(_serviceContext.UserId);
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

        public async Task<bool> ConfirmEmailAsync(string emailToken)
        {
            if (string.IsNullOrWhiteSpace(emailToken))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            JwtTokenValidator emailJwt = new JwtTokenValidator();
            ClaimParser claimParser;
            try
            {
                emailJwt.ValidateToken(emailToken);

                claimParser = new ClaimParser(emailJwt.GetAllClaims(), new List<TokenClaimPart> { new GuidTokenClaimPart(), new EmailTokenClaimPart() });
                claimParser.ParseTokenClaims();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            User? user = await _userRepository.FindByIdAsync(Guid.Parse(claimParser.TokenParts[0].Value));
            if (user == null || user.Email != claimParser.TokenParts[1].Value)
            {
                return false;
            }
            if (!user.IsConfirmed)
            {
                user.IsConfirmed = true;
                await _userRepository.UpdateAsync(user);
            }

            return true;
        }

        public async Task UpdateUserInfoAsync(string name, string surname, string nickname, string? email)
        {
            if (string.IsNullOrWhiteSpace(name) &&
               string.IsNullOrWhiteSpace(surname) &&
               string.IsNullOrWhiteSpace(nickname))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            if (name.Length > 50)
            {
                throw new ArgumentException(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (surname.Length > 50)
            {
                throw new ArgumentException(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (nickname.Length > 20)
            {
                throw new ArgumentException(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            User user = await _userRepository.FindByIdAsync(_serviceContext.UserId) ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            
            user.Name = name;
            user.Surname = surname;
            user.Nickname = nickname;
            if (email != null)
            {
                User? confirmedEmailUser = await _userRepository.GetConfirmedUserAsync(email);
                Console.WriteLine(user == confirmedEmailUser);
                if (confirmedEmailUser != null && confirmedEmailUser.Id != user.Id)
                {
                    throw new ArgumentException(ResponseErrors.EMAIL_ALREADY_EXIST);
                }

                if (email != user.Email)
                {
                    user.Email = email;
                    user.IsConfirmed = false;
                }
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateStatusAsync(string newStatus)
        {
            User user = await _userRepository.FindByIdAsync(_serviceContext.UserId) ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            user.Status = newStatus;
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(Guid? userid, string newPassword)
        {
            User user;
            if (userid.HasValue)
            {
                user = await _userRepository.FindByIdAsync(userid.Value) ?? throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            else
            {
                user  = await _userRepository.FindByIdAsync(_serviceContext.UserId) ?? throw new InvalidOperationException(ResponseErrors.USER_NOT_AUTHENTIFICATION);
            }

            string hasedPassword = Password.GetHashedPassword(newPassword);
            if (user.Password == hasedPassword)
            {
                throw new ArgumentException(ResponseErrors.PASSWORD_ALREADY_SET);
            }
            user.Password = hasedPassword;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            return await _userRepository.FindByIdAsync(_serviceContext.UserId);
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
            IEnumerable<ConfirmationCode> confirmationCodes = await _confirmationCodeRepository.GetUnusedValidCode();

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
                    await _confirmationCodeRepository.UpdateAsync(codeHash);

                    return codeHash;
                }
                catch (UnauthorizedAccessException) { }
            }

            throw new ArgumentException(ResponseErrors.INVALID_CODE);
        }

        public async Task SendCodeAsync(string email)
        {
            User user = await _userRepository.GetConfirmedUserAsync(email) ?? throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            string hashedCode;
            string generatedCode;

            do
            {
                generatedCode = CodeGenerator.Generate();
                hashedCode = Password.GetHashedPassword(generatedCode);
            }
            while ((await _confirmationCodeRepository.GetByConditions(code => code.Code == hashedCode && !code.IsUsed && !code.IsDeleted)).Any());

            ConfirmationCode code = new ConfirmationCode
            { 
                Code = hashedCode,
                DateStart = DateTime.UtcNow,
                IsUsed = false,
                UserId = user.Id
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SendToEmailAsync(user.Email, "Код восстановления", generatedCode);
            }

            await _confirmationCodeRepository.CreateAsync(code);
        }
    }
}
