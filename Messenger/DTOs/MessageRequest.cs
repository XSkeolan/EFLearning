namespace Messenger.DTOs
{
    public class MessageRequest
    {
        /// <summary>
        /// Текст отправляемого сообщения
        /// </summary>
        public string Message { get; set; } = null!;
        /// <summary>
        /// Идентификатор пункта назначения сообщения
        /// </summary>
        public Guid Destination { get; set; }
        /// <summary>
        /// Приложенные файлы к сообщению
        /// </summary>
        public IFormFileCollection Files { get; set; } = null!;
    }
}
