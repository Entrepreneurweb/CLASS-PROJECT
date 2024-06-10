using DbRepository.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Models.DTOs
{
    public class ReceiveMessageDTO
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
       
        public string? ReceiverId { get; set; } = string.Empty;
       
    }
}
