using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
using X.PagedList;
using MvcVendingMachine.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcVendingMachine.ViewModel
{
    public class PembayaranViewModel
    {
        public int Id { get; set; }


        [DisplayName("Nama Produk")]
        public string Namaproduk { get; set; }

        public int Stock { get; set; }

        public int Stockout { get; set; }

        [DisplayName("Harga Produk")]
        public int Hargaproduk { get; set; }

        
        public decimal nominal { get; set; }
        public decimal totalNominal { get; set; }

        
    }
}
