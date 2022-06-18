namespace Messenger.DTOs
{
    public class UserUpdateRequest
    {
        /// <summary>
        /// Новое имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Новая фамилия пользователя
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Новый никнейм пользователя
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// Новый Email
        /// </summary>
        public string Email { get; set; }
    }
}
