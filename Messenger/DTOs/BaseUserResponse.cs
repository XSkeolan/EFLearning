namespace Messenger.DTOs
{
    public class BaseUserResponse
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Ник пользователя
        /// </summary>
        public string Nickname { get; set; } = null!;
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; set; } = null!;
    }
}
