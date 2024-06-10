using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Models.DTOs
{
    public class BookDTO
    {
        public string Title { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public string? ImagePath { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int ISBN { get; set; }
        public float Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
       // public DateTime PublicationDate { get; set; }
        public int Stock { get; set; }


    }
}
