using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDAL.Models
{
    public class MessageFile : EntityBase
    {
        public Guid MessageId { get; set; }
        public Message? Message { get; set; }

        public Guid FileId { get; set; }
        public File? File { get; set; }
    }
}
