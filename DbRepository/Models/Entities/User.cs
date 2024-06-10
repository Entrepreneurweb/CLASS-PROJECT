using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Models.Entities
{
    public class User : IdentityUser
    {
         public  override string Id { get; set; }
        //  public string ?Name { get; set; }
        public  override string? UserName { get; set; }
        public string? Password { get; set; }
        public string ?UserEmail { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Message>? Message { get; set; }

    }
}
