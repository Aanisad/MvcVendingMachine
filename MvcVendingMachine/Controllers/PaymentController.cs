using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcVendingMachine.Controllers;
using MvcVendingMachine.Data;
using MvcVendingMachine.Models;
using MvcVendingMachine.ViewModel;

namespace MvcVendingMachine.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MvcVendingMachineContext _context;

        public PaymentController(MvcVendingMachineContext context)
        {
            _context = context;
        }


        //cek saldo
        public IActionResult Ceksaldo()
        {
            var saldo = _context.Pembayaran.ToList();
            return View(saldo);
        }


        //Topup
        public IActionResult Topup(PembayaranViewModel vm)
        {
            //penambahan semua nominal
            var data = _context.Pembayaran;
            var totalNominal = _context.Pembayaran.Sum(i => i.nominal);

            //memanggil total nominal modelview
            if (data.Count() > 0)
            {
                vm.totalNominal = totalNominal;
            }
            else
            {
                vm.totalNominal = 0;
            }
            
            return View(vm);
        }

        //POST Topup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTopup(PembayaranViewModel vm)
        {
            var data = _context.Pembayaran;

            if (data.Count() > 0)
            {
                foreach (var item in _context.Pembayaran.ToList())
                {
                    item.nominal = _context.Pembayaran.Sum(i => i.nominal
                    );
                    decimal total = item.nominal;

                    decimal topup = vm.nominal;
                    decimal totalharga = total + topup;

                    item.nominal = totalharga;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Topup");
                }
            }
            else if(data.Count() == 0)
            {
                //simpan saldo baru
                Pembayarann pembayaran = new Pembayarann();

                if (ModelState.IsValid)
                {
                    pembayaran.nominal = vm.nominal;
                    _context.Pembayaran.Add(pembayaran);
                    _context.SaveChanges();
                    return RedirectToAction("Topup");
                }
            }

            vm.totalNominal = _context.Pembayaran.Sum(i => i.nominal);

            return View("Topup",vm.totalNominal);
        }
        
    }
}
