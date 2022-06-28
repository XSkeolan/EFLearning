namespace Messenger.DTOs
{
    public class MessageResponse
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// Идентификатор пользователя-отправителя
        /// </summary>
        public Guid FromId { get; set; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Message { get; set; } = null!;
        /// <summary>
        /// Дата отправки сообщения
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Закреплено ли сообщение
        /// </summary>
        public bool IsPinned { get; set; }
        /// <summary>
        /// Вложения к сообщению, если они присутствуют
        /// </summary>
        public IEnumerable<byte[]>? Attachment { get; set; }
    }
}
