using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryAPI.Model
{
    public class GroceryItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description  { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
