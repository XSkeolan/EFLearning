using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDAL.Models
{
    public class ChatLink : EntityBase
    {
        public bool IsOneTime { get; set; }
        public DateTime DateEnd { get; set; }

        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
