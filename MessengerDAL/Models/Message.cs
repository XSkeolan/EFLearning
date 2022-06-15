using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerDAL.Models
{
    public class Message : EntityBase
    {
        [StringLength(200)]
        public string Text { get; set; }
        public DateTime DateSend { get; set; }

        public Guid FromUserId { get; set; }
        public User? FromUser { get; set; }

        public Guid DestinationChatId { get; set; }
        public Chat? DestinationChat { get; set; }

        public bool IsPinned { get; set; } = false;
        public bool IsRead { get; set; }

        public Guid? OriginalMessageId { get; set; }
        [ForeignKey("OriginalMessageId")]
        public Message? OriginalMessage { get; set; }

        public Guid? ReplyMessageId { get; set; }
        [ForeignKey("ReplyMessageId")]
        public Message? ReplyMessage { get; set; }

        public List<File> Files { get; set; } = new List<File>();
        public List<MessageFile> MessageFiles { get; set; } = new List<MessageFile>();
    }
}
