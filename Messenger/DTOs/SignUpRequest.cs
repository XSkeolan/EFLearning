namespace Messenger.DTOs
{
    public class SignUpRequest
    {
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        public string Phonenumber { get; set; }
        /// <summary>
        /// Пароль для входа
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Ник пользователя
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; set; }
    }
}
