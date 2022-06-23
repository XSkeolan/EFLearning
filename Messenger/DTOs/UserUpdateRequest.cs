namespace Messenger.DTOs
{
    public class UserUpdateRequest
    {
        /// <summary>
        /// Новое имя пользователя
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Новая фамилия пользователя
        /// </summary>
        public string Surname { get; set; } = null!;
        /// <summary>
        /// Новый никнейм пользователя
        /// </summary>
        public string NickName { get; set; } = null!;
        /// <summary>
        /// Новый Email
        /// </summary>
        public string? Email { get; set; }
    }
}
