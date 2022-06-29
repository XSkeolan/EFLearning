using System.Security.Claims;

namespace MessengerLibrary
{
    public abstract class TokenClaimPart
    {
        protected string _value;

        public TokenClaimPart()
        {
            _value = string.Empty;
        }

        public virtual string Value => _value == string.Empty ? throw new InvalidOperationException("This token part is not parsed") : _value;

        public virtual void Parse(Claim claim)
        { 
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
        }
    }
}
