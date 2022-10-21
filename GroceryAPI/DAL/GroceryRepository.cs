using GroceryAPI.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Xml.Linq;

namespace GroceryAPI.DAL
{
    public class GroceryRepository : IGroceryRepository
    {
        private readonly GroceryDbContext context;
        public GroceryRepository(GroceryDbContext context)
        {
            this.context = context;

            if (context.Items.Any())
            {
                return;
            }

            var groceryItems = new List<GroceryItem>()
            {
                new GroceryItem
                {
                    Name = "Flour",
                    Description = "1 package of 1kg",
                },
                new GroceryItem
                {
                    Name = "Pepperoni",
                    Description = "1 package of 500g",
                },
                new GroceryItem
                {
                    Name = "Tomato Sauce",
                    Description = "1 bottle",
                },
                new GroceryItem
                {
                    Name = "Mozzarella",
                    Description = "500g",
                },
            };

            context.Items.AddRange(groceryItems);
            context.SaveChanges();
        }

        public void AddItem(GroceryItem item)
        {
            context.Items.Add(item);
        }

        public void UpdateItem(GroceryItem item)
        {
            context.Items.Update(item);
        }

        public GroceryItem GetItem(int id)
        {
            return context.Items.FirstOrDefault(x => x.Id.Equals(id));
        }

        public IEnumerable<GroceryItem> GetItems()
        {
            return context.Items.ToList();
        }

        public void RemoveItem(GroceryItem item)
        {
            context.Items.Remove(item);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
