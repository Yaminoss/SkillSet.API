using Microsoft.EntityFrameworkCore;
using SkillSet.API.Models;

namespace SkillSet.API.Data
{

    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public virtual DbSet<Owner> Owners { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Owner>(owner =>
                {
                    owner.HasKey(o => o.Id);
                    owner.HasIndex(o => o.Address).IsUnique();
                }
            );
        }
    }
}