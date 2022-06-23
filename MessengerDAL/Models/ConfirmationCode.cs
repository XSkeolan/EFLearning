using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MessengerDAL.Models
{
    public class ConfirmationCode : EntityBase
    {
        [StringLength(64)]
        public string Code { get; set; } = null!;
        public DateTime DateStart { get; set; }
        public bool IsUsed { get; set; } = false;

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
