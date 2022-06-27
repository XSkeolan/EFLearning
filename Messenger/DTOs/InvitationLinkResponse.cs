namespace Messenger.DTOs
{
    public class InvitationLinkResponse
    {
        /// <summary>
        /// Идентификатор ссылки
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Токен
        /// </summary>
        public string Link { get; set; } = null!;
    }
}
