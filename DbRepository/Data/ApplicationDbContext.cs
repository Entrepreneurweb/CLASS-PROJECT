using DbRepository.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
            public  override DbSet<User> Users { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<Book> Books { get; set; }
            public DbSet<BookExtends> BookExtends { get; set; }
           //  public DbSet<BookOrder> BookOrders { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
             .HasMany(u => u.Orders)
             .WithOne(o => o.User)
             .HasForeignKey(o => o.UserID );
                 


           /* builder.Entity<Order>()
               .HasMany(o => o.Book)
               .WithOne(b => b.Order)
               .HasForeignKey(b => b.OrderID);*/

           builder.Entity<Order>()
                .HasOne( a => a.Book)
                .WithMany( b => b.Order )
                .HasForeignKey( o => o.BookID );

            /*builder.Entity<BookOrder>()
           .HasKey(bo => new { bo.BookID, bo.OrderID });

            builder.Entity<BookOrder>()
                .HasOne(bo => bo.Book)
                .WithMany(b => b.BookOrders)
                .HasForeignKey(bo => bo.BookID);

            builder.Entity<BookOrder>()
                .HasOne(bo => bo.Order)
                .WithMany(o => o.BookOrders)
                .HasForeignKey(bo => bo.OrderID);
            */
          

           builder.Entity<User>()
                .HasMany( a => a.Message )
                .WithOne( b => b.User )
                .HasForeignKey( c => c.UserId );

        }
    }
}
