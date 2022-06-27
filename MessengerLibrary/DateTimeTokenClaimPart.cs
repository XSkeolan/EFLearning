using System.Security.Claims;

namespace MessengerLibrary
{
    public class DateTimeTokenClaimPart : TokenClaimPart
    {
        public override string Type => "DateTime";

        public DateTimeTokenClaimPart() : base() { }

        public override void Parse(Claim claim)
        {
            base.Parse(claim);

            if (!DateTime.TryParse(claim.Value, out DateTime dateTime))
            {
                throw new ArgumentException("Value is not parsed");
            }

            _value = dateTime.ToString();
        }
    }
}
