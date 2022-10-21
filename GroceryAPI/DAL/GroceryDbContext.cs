using GroceryAPI.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace GroceryAPI.DAL
{
    public class GroceryDbContext : DbContext
    {
        public GroceryDbContext(DbContextOptions<GroceryDbContext> options) : base(options)
        {

        }

        public DbSet<GroceryItem> Items { get; set; }
    }
}
