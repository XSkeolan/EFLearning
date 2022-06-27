using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MessengerLibrary
{
    public class JwtTokenValidator
    {
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly List<Claim> _claims;

        public JwtTokenValidator()
        {
            _jwtHandler = new JwtSecurityTokenHandler();
            _claims = new List<Claim>();
        }

        public void ValidateToken(string token)
        {
            JwtSecurityToken parseToken = _jwtHandler.ReadJwtToken(token);

            foreach (Claim claim in parseToken.Claims)
            {
                if (string.IsNullOrWhiteSpace(claim.Value))
                {
                    throw new ArgumentException("Token is invalid");
                }
                if (_claims.Select(c => c.Type).Contains(claim.Type))
                {
                    throw new InvalidOperationException("Claim type already exist");
                }
                _claims.Add(claim);
            }
        }

        public Claim GetClaim(string type)
        {
            return _claims.FirstOrDefault(claim => claim.Type == type) ?? throw new ArgumentException("Claim value with this type not found");
        }

        public List<Claim> GetAllClaims() => _claims;
    }
}
