using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NCKH.HTMLSteg.Models.Dictionary
{
    [Table("XMLKeys")]
    public class XMLKey : BaseClass
    {
        [Key]
        public int ID { get; set; }
        [Required, MaxLength(255)]
        public string KeyName { get; set; }
        [Required]
        [MaxLength(255)]
        public string FilePath { get; set; }
        public bool isPrivate { get; set; }
    }
}