using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcVendingMachine.ViewModel
{
    public class MachineViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Nama Produk")]
        public string? Namaproduk { get; set; }

        [Required]
        public int Stock { get; set; }

        public int Stockout { get; set; }

        [Required]
        [DisplayName("Harga Produk")]
        public int Hargaproduk { get; set; }

        public decimal totalNominal { get; set; }
        public int stockbarang { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal nominal { get; set; }

    }
}
