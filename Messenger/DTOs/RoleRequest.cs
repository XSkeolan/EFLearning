namespace Messenger.DTOs
{
    public class RoleRequest
    {
        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public Guid ChatId { get; set; }
        /// <summary>
        /// Идентификатор пользователя в чате
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Идентификатор новой роли для пользователя
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
