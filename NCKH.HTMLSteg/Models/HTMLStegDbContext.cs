using NCKH.HTMLSteg.Models.Dictionary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace NCKH.HTMLSteg.Models
{
    public class HTMLStegDbContext : DbContext
    {
        public HTMLStegDbContext() : base("HTMLStegConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ReturnPage> ReturnPages { get; set; }
        public DbSet<XMLKey> XMLKeys { get; set; }
        public DbSet<Menu> Menus { get; set; }
    }
}