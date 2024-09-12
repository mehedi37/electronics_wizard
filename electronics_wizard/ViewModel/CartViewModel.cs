using electronics_wizard.Models;

namespace electronics_wizard.ViewModel
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemsModel> CartItems { get; set; } = new List<CartItemsModel>();
    }
}
