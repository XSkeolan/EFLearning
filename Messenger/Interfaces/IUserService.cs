using MessengerDAL.Models;

namespace Messenger.Interfaces
{
    public interface IUserService
    {
        public int SessionExpires { get; }
        public int EmailLinkExpires { get; }
        Task CreateUserAsync(User user);
        Task DeleteUserAsync(string reason);
        Task<User> GetUserAsync(Guid id);
        Task UpdateUserInfoAsync(string name, string surname, string nickName, string? email);
        Task UpdateStatusAsync(string newStatus);
        Task ChangePasswordAsync(Guid? userid, string newPassword);
        Task<User?> GetCurrentUserAsync();

        Task<Session> SignInAsync(string phone, string password, string device);
        Task SignOutAsync();
        Task SendToEmailAsync(string email, string subject, string content);
        Task<bool> ConfirmEmailAsync(string emailToken);
        Task<ConfirmationCode> TryGetCodeInfoAsync(string code);
        Task SendCodeAsync(string email);
    }
}