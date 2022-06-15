using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDAL.Models
{
    public class UserChat : EntityBase
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }

        public Guid UserTypeId { get; set; }
        public UserType? UserType { get; set; }
    }
}
