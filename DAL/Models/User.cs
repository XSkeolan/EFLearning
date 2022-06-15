using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    //Аналогично применению конфигурации в контексте
    //[EntityTypeConfiguration(typeof(CustomUserConfiguration))]
    //Создание индексов через атрибуты (полей может быть несколько)
    //[Index("Email", IsUnique = true, Name = "IX_Email")]
    public class User : EntityBase
    {
        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; } = null!;

        [Required]
        [MaxLength(64)]
        public string Password { get; set; } = null!;

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; } = null;

        [MaxLength(11)]
        [Phone]
        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        public bool IsConfirmed { get; set; } = false;

        public List<Session> Sessions { get; set; } = new List<Session>(); // навигационное свойство для отношения один-ко-многим

        public UserProfile? Profile { get; set; }

        public override string ToString()
        {
            return Id.ToString() + " " + Nickname + (IsDeleted ? " Удален" : " Не удален");
        }
    }
}
