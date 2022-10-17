using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using MvcVendingMachine.Data;
using System.Data;

namespace MvcVendingMachine.Controllers
{
    public class RolesController : Controller
    {
        private readonly MvcVendingMachineContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public RolesController(MvcVendingMachineContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }


        [HttpGet]
        public IActionResult Upsert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                //Update
                //Ambil ID Users
                var objFromDb = _db.Roles.FirstOrDefault(u => u.Id == id);
                return View(objFromDb);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IdentityRole roleObj)
        {
            if (await _roleManager.RoleExistsAsync(roleObj.Name))
            {
                //error
                TempData[SD.Error] = "Role already exist";
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrEmpty(roleObj.Id))
            {
                //create
                await _roleManager.CreateAsync(new IdentityRole() { Name = roleObj.Name });
                TempData[SD.Success] = "Role Created successfully";
            }
            else
            {
                //update
                var objRoleFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleObj.Id);
                if (objRoleFromDb == null)
                {
                    TempData[SD.Error] = "Role not found";
                    return RedirectToAction(nameof(Index));
                }
                objRoleFromDb.Name = roleObj.Name;
                objRoleFromDb.NormalizedName = roleObj.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(objRoleFromDb);
                TempData[SD.Success] = "Role Updated successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        //Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var objFromDb = _db.Roles.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                //error
                TempData[SD.Error] = "Role not found";
                return RedirectToAction(nameof(Index));
            }

            var userRolesForThisRole = _db.UserRoles.Where(u => u.RoleId == id).Count();
            if (userRolesForThisRole > 0)
            {
                //error
                TempData[SD.Error] = "Cant delete this role";
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(objFromDb);
            TempData[SD.Success] = "Role Deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
