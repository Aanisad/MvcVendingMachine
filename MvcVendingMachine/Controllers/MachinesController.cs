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

namespace MvcVendingMachine.Controllers
{
    public class MachinesController : Controller
    {
        private readonly MvcVendingMachineContext _context;

        public MachinesController(MvcVendingMachineContext context)
        {
            _context = context;
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
            ViewData["Mesin"] = Mesin;
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
                items.Add(vm);
            }
            
            //variable wajib 4
            //return data sebagai penampung items dan semua variable
            IPagedList<PembayaranViewModel> returnData = new StaticPagedList<PembayaranViewModel>(items, pageNumber, pageSize, totalItems);

            //menampilkan listproduct dan returndta 
            return View("ListProduct", returnData);
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
        

        //tampilan Create / tambah barang baru = hanya bisa dilihat oleh admin
        [Authorize(Roles = "Admin")]
        //GET: Machines/Create
        public IActionResult Create()
        {
            //mengambil data barang
            var Mesin = _context.Machine;
            ViewData["Mesin"] = Mesin;
            return View();

        }

        //Create / tambah barang baru = hanya bisa dilakukan oleh admin
        [Authorize(Roles = "Admin")]
        // POST: Machines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Namaproduk,Stock,Hargaproduk")] Models.Machine machine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machine);
                await _context.SaveChangesAsync();

                TempData[SD.Success] = "Sukses di input";
                return RedirectToAction(nameof(Create));
            }

            return View("Create");
        }

        //TView beli barang
        // GET: Machines/Beli/5
        [HttpPost]
        public async Task<IActionResult> Beli(MachineViewModel vm)
        {
            //Jika stock barang 0 
            var data = _context.Machine;
            int stockbarang = _context.Machine.Sum(i => i.Stock);
            //memanggil total nominal modelview
            if (data.Count() > 0)
            {
                //tidak bisa dibeli
            }
            else
            {
                //bisadibeli 
            }

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
            return RedirectToAction(nameof(Create));
        }

        private bool MachineExists(int id)
        {
            return _context.Machine.Any(e => e.Id == id);
        }

        //Belibtn
        public async Task<IActionResult> Belibtn(int? id, MachineViewModel vm)
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

        //View edit barang - hanya dapat dilakukan oleh Admin
        // GET: MvcVendingMachines/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Machine == null)
            {
                return NotFound();
            }

            var Machine = await _context.Machine.FindAsync(id);
            if (Machine == null)
            {
                return NotFound();
            }
            return View(Machine);
        }

        //Edit barang - hanya bisa dilakukan oleh Admin
        // POST: MvcVendingMachines/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Namaproduk,Stock,Hargaproduk")] Machine machine)
        {
            if (id != machine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineExists(machine.Id))
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
            return View(machine);
        }

        

    }
}
