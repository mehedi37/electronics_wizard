using electronics_wizard.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace electronics_wizard.Models
{
    public class Electronics
    {
        [Key]
        public required int ElectronicId { get; set; }
        public required string ElectronicName { get; set; }
        public required string ElectronicModel { get; set; }
        public string Image { get; set; } = string.Empty;
        [DataType(DataType.MultilineText)]
        public required string Description { get; set; }
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        [ForeignKey("AppUserUser")]
        public required string UserId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public AppUserUser? AppUserUser { get; set; }
    }
}
