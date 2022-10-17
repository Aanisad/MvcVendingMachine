using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MvcVendingMachine.Data;
using MvcVendingMachine.Models;
using MvcVendingMachine.ViewModel;
using System.Data;
using System.Security.Claims;

namespace MvcVendingMachine.Controllers
{
    public class UserController : Controller
    {
        private readonly MvcVendingMachineContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(MvcVendingMachineContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var userList = _db.ApplicationUsers.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach(var user in userList)
            {
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if(role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }
            return View(userList);
        }

        public IActionResult Edit(string userId)
        {
            //ambil user dari database
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if(objFromDb == null)
            {
                return NotFound();
            }

            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if(role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(objFromDb);
        }


        //Edit role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                //ambil user dari database
                var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }

                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == user.Id);
                if (userRole != null)
                {
                    //memanggil nama yang sudah ada role nya
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();

                    //menghapus role sebelumnya
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);


                }
                //menambahkan role baru 
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                objFromDb.Name = user.Name;
                _db.SaveChanges();
                TempData[SD.Success] = "Berhasil di Edit";
                return RedirectToAction(nameof(Index));
            }
           

            user.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }


        //Locked User
        [HttpPost]
        public IActionResult lockUnlock(string userId)
        {
            //Ambil data userid di database
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if(objFromDb == null)
            {
                return NotFound();
            }
           
            if(objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //akan terlocked sampai batas waktu yang ditentukan
                //klik ini, akan unlocked 
                objFromDb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "Berhasil dibuka kembali";
            }
            else
            {
                //user tidak terlocked, mau lock user
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "Berhasil terkunci";
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //Hapus user    
        [HttpPost]
        public IActionResult Delete (string userId)
        {
            //mengambil Id user 
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if(objFromDb == null)
            {
                return NotFound();
            }
            _db.ApplicationUsers.Remove(objFromDb);
            _db.SaveChanges();
            TempData[SD.Success] = "Berhasil dihapus";
            return RedirectToAction(nameof(Index));
        }

        //claims
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel()
            {
                UserId = user.Id,
            };
                    
            foreach(Claim claim in ClaimStore.claimsList)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c=> c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel vm)
        {
            IdentityUser user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
            {
                return NotFound();
            }

            //ambil data claim
            var claims  = await _userManager.GetClaimsAsync(user);
            var result  = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                TempData[SD.Error] = " gagal hapus claims";
                return View(vm);
            }

            result = await _userManager.AddClaimsAsync(user,
                vm.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()))
                );

            if (!result.Succeeded)
            {
                TempData[SD.Error] = " gagal menambahkan claims";
                return View(vm);
            }

            TempData[SD.Success] = "Claim berhasil diupdate";
            return RedirectToAction(nameof(Index));

        }
    }
}
