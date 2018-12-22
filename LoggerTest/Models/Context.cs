using System;
using Microsoft.EntityFrameworkCore;
using Raveshmand.Logger.Extentions;

namespace LoggerTest.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyLogConfiguration();
        }
    }
}
