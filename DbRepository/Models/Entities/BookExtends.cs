using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Models.Entities
{
    public class BookExtends
    {
        [Key]
        public int BookIOrderModelD { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int ISBN { get; set; }
        public int OrderID { get; set; }
        public float Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public Order? Order { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Number { get; set; }
    }
}
