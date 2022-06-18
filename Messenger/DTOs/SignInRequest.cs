namespace Messenger.DTOs
{
    public class SignInRequest
    {
        /// <summary>
        /// Номер телефона для входа
        /// </summary>
        public string Phonenumber { get; set; }
        /// <summary>
        /// Пароль указанный при регистрации
        /// </summary>
        public string Password { get; set; }
    }
}