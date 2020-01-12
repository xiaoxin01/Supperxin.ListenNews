using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Supperxin.ListenNews.Models;

namespace Supperxin.ListenNews.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Supperxin.ListenNews.Models.Item> Item { get; set; }
        public DbSet<Supperxin.ListenNews.Models.ListenHistory> ListenHistory { get; set; }
        public DbSet<Supperxin.ListenNews.Models.Favorite> Favorite { get; set; }
        public DbSet<Supperxin.ListenNews.Models.ViewHistory> ViewHistory { get; set; }
    }
}
