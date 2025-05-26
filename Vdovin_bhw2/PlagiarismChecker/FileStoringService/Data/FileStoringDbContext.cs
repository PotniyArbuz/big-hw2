using FileStoringService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FileStoringService.Data
{
    public class FileStoringDbContext : DbContext
    {
        public DbSet<FileEntity> Files { get; set; }

        public FileStoringDbContext(DbContextOptions<FileStoringDbContext> options) : base(options) { }
    }
}