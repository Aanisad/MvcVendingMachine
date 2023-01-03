using MessagePack;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;

namespace MvcVendingMachine.Models
{
    public class Machine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Nama Produk")]
        public string? Namaproduk { get; set; }
        public int Stock { get; set; }

        [DisplayName("Harga Produk")]
        public int Hargaproduk { get; set; }

        public string Gambar { get; set; }

        public ICollection<Images> images { get; set; }
    }
}
