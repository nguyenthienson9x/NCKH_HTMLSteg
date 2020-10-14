using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NCKH.HTMLSteg.Models.Dictionary
{
    [Table("Menus")]
    public class Menu : BaseClass
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
    }
}