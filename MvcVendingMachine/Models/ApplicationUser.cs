using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MvcVendingMachine.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string RoleId { get; set; }

        [NotMapped]
        public string Role { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
