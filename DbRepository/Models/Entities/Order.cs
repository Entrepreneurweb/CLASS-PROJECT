using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DbRepository.Models.Entities
{
    public class Order
    {
        [Key]  
        public int OrderID { get; set; }
        public string? UserID { get; set; }
        public User? User { get; set; }
        
        
        public int BookID { get; set; }
        public Book? Book { get; set; }

        public DateTime OrderDate { get; set; }
        public int OrderStatus { get; set; }

        public int Quantity { get; set; }
        public float Price { get; set; }
       // public List<BookOrder>? BookOrders { get; set; }
    }
}
