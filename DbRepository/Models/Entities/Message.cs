using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DbRepository.Models.Entities
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string MessageText { get; set; }=string.Empty;
        public string ? UserId { get; set; } = string.Empty;
        public User? User { get; set; }
        public string? ReceiverId { get; set; } = string.Empty;
       
    }
}
