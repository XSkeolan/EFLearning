using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Session : EntityBase
    {
        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeviceName { get; set; } = null!;

        [Required]
        public DateTime DateEnd { get; set; }
        // альтернативный внешний ключ (могут быть взаимозаменяемы)
        //public string? Name { get; set; }
        // внешний ключ
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }  // навигационное свойство
    }
}
