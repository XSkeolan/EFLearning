namespace Messenger.DTOs
{
    public class UserFullResponse : BaseUserResponse
    {
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        public string Phonenumber { get; set; } = null!;
        /// <summary>
        /// Статус пользователя
        /// </summary>
        public string Status { get; set; } = null!;
        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Подтвержден ли email
        /// </summary>
        public bool IsConfirmed { get; set; }
    }
}
