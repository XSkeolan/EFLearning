using System.Security.Claims;

namespace MessengerLibrary
{
    public class GuidTokenClaimPart : TokenClaimPart
    {
        public GuidTokenClaimPart() : base() { }

        public override void Parse(Claim claim)
        {
            base.Parse(claim);

            if (!Guid.TryParse(claim.Value, out Guid guid))
            {
                throw new ArgumentException("Value is not parsed");
            }

            _value = guid.ToString();
        }
    }
}
