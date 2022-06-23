namespace Messenger.DTOs
{
    public class SignUpRequest
    {
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        public string Phonenumber { get; set; } = null!;
        /// <summary>
        /// Пароль для входа
        /// </summary>
        public string Password { get; set; } = null!;
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
