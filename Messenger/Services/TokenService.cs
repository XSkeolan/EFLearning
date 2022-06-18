using Messenger.Options;
using MessengerDAL.Models;
using MessengerDAL;
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

        private readonly MessengerContext _messengerContext;
        private readonly IServiceContext _serviceContext;

        public TokenService(IOptions<JwtOptions> options, MessengerContext messengerContext, IServiceContext serviceContext)
        {
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _key = options.Value.Key;
            _sessionExpires = options.Value.SessionExpires;
            _emailLinkExpires = options.Value.EmailLinkExpires;

            _messengerContext = messengerContext;
            _serviceContext = serviceContext;
        }

        public async Task<string> CreateSessionToken(Guid sessionId)
        {
            Session? session = await _messengerContext.Sessions.FindAsync(sessionId);
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

        public async Task<string> CreateEmailToken()
        {
            User? user = await _messengerContext.Users.FindAsync(_serviceContext.UserId);
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

        public async Task<string> CreateInvitationToken(Guid channelLinkId)
        {
            ChatLink? channelLink = await _messengerContext.ChatLinks.FindAsync(channelLinkId);
            if (channelLink == null)
            {
                throw new ArgumentNullException(ResponseErrors.CHANNEL_LINK_NOT_FOUND);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, channelLinkId.ToString(), "Guid"),
                new Claim("DateEnd", channelLink.DateEnd.ToString(), "DateTime")
            };

            return CreateJwtToken(claims, channelLink.DateEnd);
        }

        private string CreateJwtToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var jwt = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
