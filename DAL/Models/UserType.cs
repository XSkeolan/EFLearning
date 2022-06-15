using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    // Указывает, что свойство или класс не отслеживается в модели БД
    [NotMapped]
    public class UserType : EntityBase
    {
        [Required]
        [MaxLength(30)]
        public string TypeName { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Permissions { get; set; } = null!;

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public byte PriorityLevel { get; set; }
    }
}
