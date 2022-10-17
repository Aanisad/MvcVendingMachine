using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcVendingMachine.Models;

namespace MvcVendingMachine.Data
{
    public class MvcVendingMachineContext : IdentityDbContext<IdentityUser>
    {
        internal static object ListProduct;

        public MvcVendingMachineContext(DbContextOptions<MvcVendingMachineContext> options) : base(options)
        {

        }

        public static object ShortLinks { get; internal set; }
        public DbSet<Machine> Machine { get; set; }
        public DbSet<Pembayarann> Pembayaran { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    }
}
