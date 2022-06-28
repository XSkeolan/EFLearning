namespace Messenger.DTOs
{
    public class ReplyMessageRequest
    {
        /// <summary>
        /// Идентификато сообщения на который нужно ответить
        /// </summary>
        public Guid ReplyMessageId { get; set; }
        /// <summary>
        /// Сообщение в ответ
        /// </summary>
        public string ReplyMessage { get; set; } = null!;
    }
}
