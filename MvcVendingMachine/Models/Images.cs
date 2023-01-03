using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcVendingMachine.Models
{
    public class Images
    {
        [Key]
        public int IdImage { get; set; }
        public string ImagePath { get; set; }
      
        [ForeignKey("Machine")]
        public int Id { get; set; }

        public Machine Machine { get; set; }


    }
}
