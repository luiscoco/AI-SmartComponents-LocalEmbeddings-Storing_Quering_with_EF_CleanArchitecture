using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComponents_Storing_Quering_with_EF_CleanArchitecture
{
    // Database context for Entity Framework Core
    public class MyDbContext : DbContext
    {
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use SQLite database
            optionsBuilder.UseSqlite("Data Source=documents.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasKey(d => d.DocumentId);
        }
    }
}
