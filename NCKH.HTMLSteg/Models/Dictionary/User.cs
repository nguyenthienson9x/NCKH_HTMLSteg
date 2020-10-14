using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NCKH.HTMLSteg.Models.Dictionary
{
    [Table("Users")]
    public class User:BaseClass
    {
        [Key]
        public int ID { get; set; }

        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
        [MaxLength(50)]
        public string UserRole { get; set; }

        public int? XMLKeyID { get; set; }
        [ForeignKey("XMLKeyID")]
        public virtual XMLKey XMLKey { get; set; }


        public int? ReturnPageID { get; set; }
        [ForeignKey("ReturnPageID")]
        public virtual ReturnPage ReturnPage { get; set; }
    }
}