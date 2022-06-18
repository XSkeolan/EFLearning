namespace Messenger.DTOs
{
    public class ChatRequest
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Идентификатор фотографии
        /// </summary>
        public Guid? PhotoId { get; set; }
        /// <summary>
        /// Идентификатор типа пользователя по умолчанию при инвайте
        /// </summary>
        public Guid DefaultUserTypeId { get; set; }
    }
}