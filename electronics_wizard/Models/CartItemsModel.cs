using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electronics_wizard.Models
{
    public class CartItemsModel
    {
        [Key]
        public required int CartItemsId { get; set; }
        [ForeignKey("Electronics")]
        public required int ElectronicId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Electronics? Electronics { get; set; }

        [Precision(18, 2)]
        public required decimal Price { get; set; }
        [ForeignKey("CartModel")]
        public required int CartId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public CartModel? Cart { get; set; }
        public required int Quantity { get; set; }
    }
}
