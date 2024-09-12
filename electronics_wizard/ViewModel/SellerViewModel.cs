using electronics_wizard.Models;

namespace electronics_wizard.ViewModel
{
    public class SellerViewModel
    {
        public List<Electronics> ItemsForSale { get; set; } = new List<Electronics>();
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    }
}
