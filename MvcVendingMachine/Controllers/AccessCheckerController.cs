using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcVendingMachine.Controllers
{

    [Authorize]
    public class AccessCheckerController : Controller
    {

        [AllowAnonymous]
        //dapat di akses oleh semua orang, bahkan ketika sedang tidak login 
        public IActionResult AllAccess()
        {
            return View();
        }

        [Authorize]
        //hanya dapat diakses oleh yang sudah di authorized
        public IActionResult AuthorizedAccess()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        //hanya user role yang dapat akses
        public IActionResult UserAccess()
        {
            return View();
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult UserorAdminAccess()
        {
            return View();
        }

        [Authorize(Roles = "UserANDAdmin")]
        public IActionResult UserANDAdminAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin")]
        //hanya admin role yang dapat akses
        public IActionResult AdminAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin_CreateAccess")]
        //dapat diakses oleh admin users (claims create)
        public IActionResult Admin_CreateAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin_Create_Edit_DeleteAccess")]
        //dapat diakses oleh admin user (claims create, edit dan delete)
        public IActionResult Admin_Create_Edit_DeleteAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin_Create_Edit_DeleteAccess_OR_SuperAdmin")]
        //dapat diakses oleh admin user (claims create, edit dan delete)/superadmin
        public IActionResult Admin_Create_Edit_DeleteAccess_OR_SuperAdmin()
        {
            return View();
        }
}
}
