using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDAL.Models
{
    public class Session : EntityBase
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        [StringLength(200)]
        public string DeviceName { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
