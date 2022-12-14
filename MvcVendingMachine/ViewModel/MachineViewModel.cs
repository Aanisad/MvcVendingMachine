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

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal nominal { get; set; }
        public IFormFile Gambar { get; set; }

    }
}
