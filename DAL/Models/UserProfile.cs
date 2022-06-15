using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserProfile
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; } = null!;

        [MaxLength(50)]
        public string? Reason { get; set; } = null!;

        [MaxLength(255)]
        public string? Status { get; set; } = null!;

        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}
