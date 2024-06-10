using DbRepository.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Models.DTOs
{
    public class OrderDTO
    {

        
        public int BookID { get; set; } 
        

        public int Quantity { get; set; }
        public int OrderStatus { get; set; }
        public float Price { get; set; }
       
        
        
    }
}
