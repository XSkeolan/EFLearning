using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDAL.Models
{
    public class Chat : EntityBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }

        public Guid CreatorId { get; set; }
        public User? Creator { get; set; }

        public Guid? PhotoId { get; set; }
        public File? Photo { get; set; }

        public Guid DefaultUserTypeId { get; set; }
        public UserType? DefaultUserType { get; set; }

        //public List<User> Users { get; set; } = new List<User>();
        public List<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}
