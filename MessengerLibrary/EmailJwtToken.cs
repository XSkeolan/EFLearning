using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MessengerLibrary
{
    public class EmailJwtToken
    {
        JwtSecurityTokenHandler _jwtHandler;
        Claim[] _claims;

        public EmailJwtToken()
        {
            _jwtHandler = new JwtSecurityTokenHandler();
            _claims = new Claim[2];
        }

        public void ValidateToken(string token)
        {
            JwtSecurityToken parseToken = _jwtHandler.ReadJwtToken(token);

            _claims[0] = parseToken.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType && claim.ValueType == "Guid") 
                ?? throw new ArgumentException("Invalid claim");
            _claims[1] = parseToken.Claims.FirstOrDefault(claim => claim.Type == "Email") 
                ?? throw new ArgumentException("Invalid claim");
        }

        public Claim[] Claims => _claims;
    }
}
