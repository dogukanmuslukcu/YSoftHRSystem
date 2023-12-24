using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace YSoftHrSystem.Models
{
    public class ReCapProjectDBContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=YSoftDB;Trusted_Connection=true");
        }


        public DbSet<User> Users { get; set; }
       

    }
}