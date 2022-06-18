using Messenger.Interfaces;
using MessengerDAL;
using MessengerDAL.Models;

namespace Messenger.Services
{
    public class LinkService : ILinkService
    {
        private readonly IServiceContext _serviceContext;
        private readonly MessengerContext _messengerContext;

        public LinkService(MessengerContext messengerContext, IServiceContext serviceContext)
        {
            _messengerContext = messengerContext;
            _serviceContext = serviceContext;
        }

        public async Task<string> GetEmailLink(string emailToken)
        {
            User? user = await _messengerContext.Users.FindAsync(_serviceContext.UserId);
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException(ResponseErrors.USER_EMAIL_NOT_SET);
            }

            return "api/auth/confirm" + "?e=" + emailToken;
        }
    }
}
