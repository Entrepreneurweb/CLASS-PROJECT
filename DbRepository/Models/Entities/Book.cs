using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DbRepository.Models.Entities
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty  ;
        public string Author { get; set; }  = string.Empty;
        public int ISBN { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }  = string.Empty;
        public string Genre { get; set; } = string.Empty;
        //public DateTime PublicationDate { get; set; }
        public string PublicationDate { get; set; }=string.Empty;
        public int Stock { get; set; }
        //public List<BookOrder>? BookOrders { get; set; }
        public List<Order>?  Order { get; set; }
    }
}
