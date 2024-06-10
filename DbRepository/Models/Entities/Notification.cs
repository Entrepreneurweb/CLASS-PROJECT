using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Models.Entities
{
    public  class Notification
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; }=string.Empty;
        public string IssuedDate { get; set; } = string.Empty ;

    }
}
