namespace Messenger.DTOs
{
    public class KickUserRequest
    {
        /// <summary>
        /// Идентификатор чата из которого нужно кикнуть пользователя
        /// </summary>
        public Guid ChatId { get; set; }
        /// <summary>
        /// Идентификатор пользователя которого нужно кикнуть
        /// </summary>
        public Guid UserId { get; set; }
    }
}
