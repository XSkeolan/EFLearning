namespace Messenger.DTOs
{
    public class InviteUserRequest
    {
        /// <summary>
        /// Идентификатор чата, в который нужно пригласить пользователя
        /// </summary>
        public Guid ChatId { get; set; }
        /// <summary>
        /// Идентификатор пользователя для приглашения
        /// </summary>
        public Guid UserId { get; set; }
    }
}
