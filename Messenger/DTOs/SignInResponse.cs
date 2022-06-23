namespace Messenger.DTOs
{
    public class SignInResponse
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// Токен авторизации
        /// </summary>
        public string Token { get; set; } = null!;
        /// <summary>
        /// Время жизни токена в секундах
        /// </summary>
        public int Expiries { get; set; }
    }
}
