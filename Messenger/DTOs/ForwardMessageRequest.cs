namespace Messenger.DTOs
{
    public class ForwardMessageRequest
    {
        /// <summary>
        /// Идентификатор сообщения, которое нужно переслать
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// Идентификатор чата, куда нужно переслать сообщение
        /// </summary>
        public Guid ChatId { get; set; }
        /// <summary>
        /// Сообщение-комментарий к пересылаемому сообщению
        /// </summary>
        public string Message { get; set; } = null!;
    }
}
