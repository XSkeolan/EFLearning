namespace Messenger.Interfaces
{
    public interface ILinkService
    {
        Task<string> GetEmailLink(string emailToken);
    }
}