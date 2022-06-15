using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public abstract class EntityBase
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
