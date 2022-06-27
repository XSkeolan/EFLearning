using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDAL.Models
{
    public class UserType : EntityBase
    {
        [StringLength(30)]
        public string TypeName { get; set; }

        [StringLength(256)]
        public string? Permissions { get; set; }
        public bool IsDefault { get; set; }
        public short PriorityLevel { get; set; }

        public List<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}
