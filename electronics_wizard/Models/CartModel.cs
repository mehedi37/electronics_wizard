using electronics_wizard.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electronics_wizard.Models
{
    public class CartModel
    {
        [Key]
        public required int CartId { get; set; }
        [ForeignKey("AppUserUser")]
        public required string UserId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public AppUserUser? AppUserUser { get; set; }
        public bool IsPurchased { get; set; } = false;
        public ICollection<CartItemsModel> CartItems { get; set; } = new List<CartItemsModel>();
    }
}
