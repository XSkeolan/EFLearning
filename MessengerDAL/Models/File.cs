using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDAL.Models
{
    public class File : EntityBase
    {
        [StringLength(50)]
        public string Server { get; set; }

        [StringLength(255)]
        public string Path { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
        public List<MessageFile> MessageFiles { get; set; } = new List<MessageFile>();
    }
}
