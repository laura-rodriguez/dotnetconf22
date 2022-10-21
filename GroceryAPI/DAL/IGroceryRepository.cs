using GroceryAPI.Model;

namespace GroceryAPI.DAL
{
    public interface IGroceryRepository
    {
        GroceryItem GetItem(int id);

        IEnumerable<GroceryItem> GetItems();

        void AddItem(GroceryItem item);

        void UpdateItem(GroceryItem item);

        void RemoveItem(GroceryItem item);

        void SaveChanges();
    }
}
