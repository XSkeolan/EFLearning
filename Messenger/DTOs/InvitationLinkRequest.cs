namespace Messenger.DTOs
{
    public class InvitationLinkRequest
    {
        /// <summary>
        /// Идентификатор канала на который ведет ссылка
        /// </summary>
        public Guid ChannelId { get; set; }
        /// <summary>
        /// Является ли ссылка одноразовой
        /// </summary>
        public bool IsOneTime { get; set; }
        /// <summary>
        /// Время действия ссылки
        /// </summary>
        public string ValidTime { get; set; } = null!;
    }
}
