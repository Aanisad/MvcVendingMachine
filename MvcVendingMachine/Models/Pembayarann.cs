using MessagePack;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace MvcVendingMachine.Models
{
    public class Pembayarann
    {
        public int id { get; set; }

        [Required]
        public decimal nominal { get; set; }
    }
}
