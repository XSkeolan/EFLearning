using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MessengerLibrary
{
    public class GuidTokenClaimPart : TokenClaimPart
    {
        public override string Type => "Guid";

        public GuidTokenClaimPart()
        {
            _value = string.Empty;
        }

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
