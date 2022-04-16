using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Data
{
    public class CalendarDBContext : DbContext
    {
        public CalendarDBContext(DbContextOptions<CalendarDBContext> options) : base(options) { }

        public DbSet<tbUser> tbUsers { get; set; }
        public DbSet<tbCalendar> tbCalendars { get; set; }
        public DbSet<tbLog> tbLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
