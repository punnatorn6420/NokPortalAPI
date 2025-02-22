using Microsoft.EntityFrameworkCore;
using NokCore.Identity.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<AppEnvironment> AppEnvironments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AssignedUserApp> AssignedUserApps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>()
                .HasMany(a => a.Environments)
                .WithOne(e => e.AssociatedApp)
                .HasForeignKey(ae => ae.AppId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure that the app name is unique.
            modelBuilder.Entity<App>()
                .HasIndex(a => a.Name)
                .IsUnique();

            // Ensure that the application environment is unique.
            modelBuilder.Entity<AppEnvironment>()
                .HasIndex(ae => new { ae.AppId, ae.EnvironmentType })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId);

            // Ensure that the email is unique.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<App>()
                .HasMany(a => a.AssignedUserApps)
                .WithOne()
                .HasForeignKey(aua => aua.AppId);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.AssociatedRole)
                .HasForeignKey(ur => ur.RoleId);

            // Ensure that the role name is unique.
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.RolePrivileges)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<Privilege>()
                .HasMany(p => p.RolePrivileges)
                .WithOne()
                .HasForeignKey(rp => rp.PrivilegeId);

            // Ensure that the privilege name is unique.
            modelBuilder.Entity<Privilege>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RolePrivilege>()
                .HasKey(rp => new { rp.RoleId, rp.PrivilegeId });

            modelBuilder.Entity<AssignedUserApp>()
                .HasKey(aua => new { aua.UserId, aua.AppId, aua.AppRoleId });
        }

    }
}
