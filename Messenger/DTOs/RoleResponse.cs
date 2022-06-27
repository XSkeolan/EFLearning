namespace Messenger.DTOs
{
    public class RoleResponse
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Доступные права для роли
        /// </summary>
        public IEnumerable<string> Permissions { get; set; } = null!;
        /// <summary>
        /// Является ли она ролью по умолчанию
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// Уровень приоритета роли
        /// </summary>
        public int PriorityLevel { get; set; }
    }
}
