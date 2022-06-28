using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MessengerLibrary
{
    public class ClaimParser
    {
        private readonly List<TokenClaimPart> _tokenParts;
        private readonly List<Claim> _claims;

        public List<TokenClaimPart> TokenParts => _tokenParts;

        public ClaimParser(List<Claim> claims)
        {
            _claims = claims;
            _tokenParts = new List<TokenClaimPart>();
        }

        public ClaimParser(List<Claim> claims, List<TokenClaimPart> tokenParts)
        {
            if (claims.Count != tokenParts.Count+3)
            {
                throw new ArgumentException("The list of claims and the list of token parts must be the same in length");
            }
            _claims = claims;
            _tokenParts = tokenParts;
        }

        public void ParseTokenClaims()
        {
            for (int i = 0; i < _tokenParts.Count; i++)
            {
                _tokenParts[i].Parse(_claims[i]);
            }
        }
    }
}
