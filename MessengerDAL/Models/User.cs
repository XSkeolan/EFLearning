using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDAL.Models
{
    public class User : EntityBase
    {
        [StringLength(11)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Nickname { get; set; }

        [StringLength(64)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(250)]
        public string? Status { get; set; }

        [StringLength(50)]
        public string? Reason { get; set; }
        public bool IsConfirmed { get; set; } = false;

        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<Chat> Chats { get; set; } = new List<Chat>();
        public List<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}
