namespace Messenger.DTOs
{
    public class SignInResponse
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// Токен авторизации
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Время жизни токена в секундах
        /// </summary>
        public int Expiries { get; set; }
    }
}
