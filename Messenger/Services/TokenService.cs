using Messenger.Options;
using MessengerDAL.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Messenger.Interfaces;

namespace Messenger.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _sessionExpires;
        private readonly int _emailLinkExpires;

        private readonly IServiceContext _serviceContext;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatLinkRepository _chatLinkRepository;

        public TokenService(IOptions<JwtOptions> options, IServiceContext serviceContext, ISessionRepository sessionRepository, IUserRepository userRepository, IChatLinkRepository chatLinkRepository)
        {
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _key = options.Value.Key;
            _sessionExpires = options.Value.SessionExpires;
            _emailLinkExpires = options.Value.EmailLinkExpires;

            _serviceContext = serviceContext;
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
            _chatLinkRepository = chatLinkRepository;
        }

        public async Task<string> CreateSessionTokenAsync(Guid sessionId)
        {
            Session? session = await _sessionRepository.FindByIdAsync(sessionId);
            if (session == null)
            {
                throw new ArgumentException(ResponseErrors.SESSION_NOT_FOUND);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, session.Id.ToString(), "Guid"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };

            return CreateJwtToken(claims, session.DateStart.AddSeconds(_sessionExpires));
        }

        public async Task<string> CreateEmailTokenAsync()
        {
            User? user = await _userRepository.FindByIdAsync(_serviceContext.UserId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException(ResponseErrors.USER_EMAIL_NOT_SET);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString(), "Guid"),
                new Claim("Email", user.Email)
            };

            return CreateJwtToken(claims, DateTime.UtcNow.AddSeconds(_emailLinkExpires));
        }

        public async Task<string> CreateInvitationTokenAsync(Guid channelLinkId)
        {
            ChatLink? channelLink = await _chatLinkRepository.FindByIdAsync(channelLinkId);
            if (channelLink == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_NOT_FOUND);
            }
            //fix
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, channelLinkId.ToString(), "Guid"),
                new Claim("DateEnd", channelLink.DateEnd.ToString(), "DateTime")
            };

            return CreateJwtToken(claims, channelLink.DateEnd);
        }

        private string CreateJwtToken(IEnumerable<Claim> claims, DateTime expires)
        {
            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
