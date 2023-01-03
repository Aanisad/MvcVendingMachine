using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcVendingMachine.Data;
using MvcVendingMachine.Migrations;
using MvcVendingMachine.Models;
using MvcVendingMachine.ViewModel;
using X.PagedList;
using Machine = MvcVendingMachine.Models.Machine;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Drawing;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Drawing.Printing;
using Xunit.Abstractions;
using System.Net;
using System.Reflection.PortableExecutable;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Data.SqlClient;

namespace MvcVendingMachine.Controllers
{
    public class MachinesController : Controller
    {
        private readonly MvcVendingMachineContext _context;
        private readonly IHostingEnvironment environment;

        public MachinesController(MvcVendingMachineContext context, IHostingEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }



        // Index
        public async Task<IActionResult> Index(PembayaranViewModel vm)
        {

            ////mengambil data nominal + penambahan semua nominal
            var data = _context.Pembayaran;
            var totalNominal = _context.Pembayaran.Sum(i => i.nominal);

            //mengambil data barang
            var Mesin = _context.Machine;


            //memanggil total nominal modelview
            if (data.Count() > 0)
            {

                vm.totalNominal = totalNominal;
            }
            else
            {
                vm.totalNominal = 0;
            }

            //Viewdata table barang
            ViewData["Mesin"] = Mesin.ToList();
            return View(vm);
        }


        //Searching, fitler dan pagging pakai viewmodel
        public async Task<IActionResult> ListProduct(string sortOrder,
        string currentFilter,
        string searchString, int? page)
        {
            //mengambil data nominal + penambahan semua nominal
            var data = _context.Pembayaran;
            var totalNominal = _context.Pembayaran.Sum(i => i.nominal);

            //pagging
            var pageNumber = page ?? 1;
            int pageSize = 4; //memunculkan jumlah barang dalam 1 page

            //viewbag
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_d" : "";
            ViewData["HargaSortParm"] = sortOrder == "harga_a" ? "harga_d" : "harga_a";
            ViewData["CurrentFilter"] = searchString;

            //semua data akan ditampung di items
            IList<PembayaranViewModel> items = new List<PembayaranViewModel>();

            var product = from s in _context.Machine
                          select s;

            //searching
            if (!String.IsNullOrEmpty(searchString))
            {
                product = product.Where(s => s.Namaproduk.Contains(searchString)
                                       || s.Hargaproduk.ToString().Contains(searchString));
            }

            //Sorting
            switch (sortOrder)
            {
                case "name_d":
                    product = product.OrderByDescending(s => s.Namaproduk);
                    break;
                case "harga_a":
                    product = product.OrderBy(s => s.Hargaproduk);
                    break;
                case "harga_d":
                    product = product.OrderByDescending(s => s.Hargaproduk);
                    break;
                default:
                    product = product.OrderBy(s => s.Namaproduk);
                    break;
            }

            //memunculkan dan menghitung total keseluruhan barang.
            int totalItems = 0;
            totalItems = product.Count();

            //perulangan untuk pembayaranviewmodel dan akan di tampung di items
            foreach (var item in product.ToList().ToPagedList(pageNumber, pageSize))
            {
                PembayaranViewModel vm = new PembayaranViewModel();
                vm.Namaproduk = item.Namaproduk;
                vm.Hargaproduk = item.Hargaproduk;
                vm.Stock = item.Stock;
                //vm.Gambar = item.Gambar;
                items.Add(vm);
            }

            //variable wajib 4
            //return data sebagai penampung items dan semua variable
            IPagedList<PembayaranViewModel> returnData = new StaticPagedList<PembayaranViewModel>(items, pageNumber, pageSize, totalItems);

            //menampilkan listproduct dan returndta 
            return View("ListProduct", returnData);
        }

        //Detail barang + image
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }

            var data = _context.Machine.Where(i => i.Id == id).SingleOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            MachineImageViewModel vm = new MachineImageViewModel();
            vm.Id = data.Id;
            vm.Namaproduk = data.Namaproduk;
            vm.Hargaproduk = data.Hargaproduk;
            vm.Stock = data.Stock;
            vm.ImageString = GetImages(data.Id);
            
            return View(vm);
        }

        //POST Topup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTopup(PembayaranViewModel vm)
        {
            //Ambil data pembayaran
            var data = _context.Pembayaran;


            //jika datanya 0
            if (data.Count() > 0)
            {

                foreach (var item in _context.Pembayaran.ToList())
                {
                    //memanggil nominal
                    item.nominal = _context.Pembayaran.Sum(i => i.nominal
                    );
                    decimal total = item.nominal;

                    decimal topup = vm.nominal;
                    decimal totalharga = total + topup; //menjumlahkan nominal topup 

                    item.nominal = totalharga;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            else if (data.Count() == 0)
            {
                //simpan saldo baru
                Pembayarann pembayaran = new Pembayarann();

                if (ModelState.IsValid)
                {
                    pembayaran.nominal = vm.nominal;
                    _context.Pembayaran.Add(pembayaran);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            //viewmodel totalnominal, menampilkan nominal keselurusan
            vm.totalNominal = _context.Pembayaran.Sum(i => i.nominal);

            return View("Index", vm.totalNominal);
        }

        public List<ImagesViewModel> GetImages(int id)
        {
            List<ImagesViewModel> items = new List<ImagesViewModel>();
            var images = from i in _context.Image
                         join m in _context.Machine on i.Id equals m.Id
                         where m.Id == id
                         select new
                         {
                             m.Id,
                             m.Namaproduk,
                             m.Stock,
                             m.Hargaproduk,
                             i.IdImage,
                             i.ImagePath
                         };

            foreach (var item in images.ToList())
            {
                ImagesViewModel vm = new ImagesViewModel();
                vm.ImagePath = item.ImagePath;
                items.Add(vm);
            }

            return (items);
        }

        [HttpGet]
        public IActionResult Indexproduct()
        {
            IList<MachineImageViewModel> items = new List<MachineImageViewModel>();
            var data = from m in _context.Machine
                       join i in _context.Image on m.Id equals i.Id
                       where m.Id == i.Id
                       select new
                       {
                           m.Id,
                           m.Namaproduk,
                           m.Stock,
                           m.Hargaproduk,
                           i.IdImage,
                           i.ImagePath
                       };

            var result = data.GroupBy(x => x.Id).Select(g => g.First());

            foreach (var item in result)
            {
                MachineImageViewModel vm = new MachineImageViewModel();
                vm.Id = item.Id;
                vm.IdImage = item.IdImage;
                vm.Namaproduk = item.Namaproduk;
                vm.Hargaproduk = item.Hargaproduk;
                vm.Stock = item.Stock;
                vm.ImageString = GetImages(item.Id);
                items.Add(vm);
            }

            return View(items);
        }

        //tampilan Create / tambah barang baru = hanya bisa dilihat oleh admin
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            MachineImageViewModel vm = new MachineImageViewModel();
            ViewBag.images = new SelectList(_context.Machine.ToList(), "Id", "Namaproduk");

            return View(vm);
        }

        //Create / tambah barang baru = hanya bisa dilakukan oleh admin
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MachineImageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Machine machine = new Machine();
                machine.Namaproduk = vm.Namaproduk;
                machine.Hargaproduk = vm.Hargaproduk;
                machine.Stock = vm.Stock;

                _context.Add(machine);
                _context.SaveChanges();

                int idmachine = machine.Id;

                foreach (var item in vm.Gambar)
                {
                    String stringFileName = UploadFile(item);
                    var image = new Images
                    {

                        ImagePath = stringFileName,
                        Id = idmachine
                    };
                    _context.Image.Add(image);
                }
                //var idmachine = 
                _context.SaveChanges();
                return RedirectToAction("Indexproduct");

            }
            return View("Create");
        }



        private string UploadFile(IFormFile file)
        {
            string fileName = null;
            if (file != null)
            {
                string uploadDir = Path.Combine(environment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return fileName;
        }





        //TView beli barang
        // GET: Machines/Beli/5
        [HttpPost]
        public async Task<IActionResult> Beli(MachineViewModel vm)
        {
            decimal total = vm.totalNominal;
            var data = _context.Machine;
            int hargaproduct = vm.Hargaproduk;
            int stockbarang = _context.Machine.Sum(i => i.Stock);



            //ambil ID
            int id = vm.Id;

            if (id == null || _context.Machine == null)
            {
                return NotFound();
            }
            //barang bisa dibeli
            var machine = await _context.Machine.FindAsync(id);
            decimal totalSaldo = _context.Pembayaran.Sum(i => i.nominal);

            if (machine == null)
            {
                return NotFound();
            }

            vm.Stock = machine.Stock;
            vm.Hargaproduk = machine.Hargaproduk;
            vm.Namaproduk = machine.Namaproduk;
            vm.totalNominal = totalSaldo;
            return View(vm);
        }

        //Delete barang - hanya bisa dilakukan oleh Admin
        // GET: Machines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Machine == null)
            {
                return NotFound();
            }

            var machine = await _context.Machine
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }

        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Machine == null)
            {
                return Problem("Entity set 'MvcVendingMachineContext.Machine'  is null.");
            }
            var machine = await _context.Machine.FindAsync(id);
            if (machine != null)
            {
                _context.Machine.Remove(machine);
            }

            await _context.SaveChangesAsync();
            TempData[SD.Success] = "Berhasil di hapus";
            return RedirectToAction(nameof(Indexproduct));
        }

        private bool MachineExists(int id)
        {
            return _context.Machine.Any(e => e.Id == id);
        }

        //Belibtn
        public async Task<IActionResult> Belibtn(int? id, PembayaranViewModel vm)
        {


            //ambil ID 
            var machine = _context.Machine.Where(m => m.Id == id).Single();
            var nominal = _context.Pembayaran.Sum(i => i.nominal);

            //ambil nominal
            vm.totalNominal = nominal;

            //Ambil harga baramg
            int hargaproduk = machine.Hargaproduk;

            int resultstock = 0;

            //ambil stock
            int stock = machine.Stock;

            //nominal kurang dari harga
            if (Convert.ToInt32(nominal) < hargaproduk)
            {
                TempData[SD.Error] = " Saldo anda kurang ";
                ViewData["Mesin"] = _context.Machine.ToList();
                return View("Index", vm);
            }
            else
            {
                //Kurangin saldo (perulangan)
                foreach (var item in _context.Pembayaran.ToList())
                {
                    item.nominal = _context.Pembayaran.Sum(i => i.nominal);
                    decimal total = item.nominal;

                    decimal totalnominal = nominal - hargaproduk;

                    item.nominal = totalnominal;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }

                //Kurangin stock
                resultstock = stock - 1;

                //update stock
                machine.Stock = resultstock;
                _context.Update(machine);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(vm);

        }

        //View edit barang - hanya dapat dilakukan oleh Admin
        // GET: MvcVendingMachines/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = _context.Machine.Where(i => i.Id == id).SingleOrDefault();
         
            if (machine == null)
            {
                return NotFound();
            }
            MachineImageViewModel vm = new MachineImageViewModel();
            vm.Id = machine.Id;
            vm.Namaproduk = machine.Namaproduk;
            vm.Hargaproduk = machine.Hargaproduk;
            vm.Stock = machine.Stock;
            vm.ImageString = GetImages(machine.Id);

            return View(vm);

        }

        //Edit barang - hanya bisa dilakukan oleh Admin
        // POST: MvcVendingMachines/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editt(int? id)
        {
            var data = _context.Machine.Where(i => i.Id == id).SingleOrDefault();

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(data);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineExists(data.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Create));
            }
            return View(data);
        }

    }
}
