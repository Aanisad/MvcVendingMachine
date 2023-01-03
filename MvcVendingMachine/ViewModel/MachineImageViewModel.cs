using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using MvcVendingMachine.Models;

namespace MvcVendingMachine.ViewModel
{
    public class MachineImageViewModel
    {
        public int IdImage { get; set; }

        [DisplayName("Nama Produk")]
        public string Namaproduk { get; set; }
        public string ImagePath { get; set; }
        public int Stock { get; set; }

        [DisplayName("Harga Produk")]
        public int Hargaproduk { get; set; }

        public List<IFormFile> Gambar { get; set; }
        public int Id { get; set; }

        public List<ImagesViewModel> ImageString { get; set; }


    }
}
