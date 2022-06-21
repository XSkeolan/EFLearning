using System.Net.Mail;
using System.Security.Claims;

namespace MessengerLibrary
{
    public class EmailTokenClaimPart : TokenClaimPart
    {
        public override string Type => "Email";

        public override void Parse(Claim claim)
        {
            base.Parse(claim);

            _ = new MailAddress(claim.Value);

            _value = claim.Value;
        }
    }
}
