using Ecommercebegin.Products.WebaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommercebegin.Products.WebaAPI.Context
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;
    }
}
