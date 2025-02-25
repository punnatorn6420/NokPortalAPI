using Microsoft.EntityFrameworkCore;
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
        public DbSet<Permission> Privileges { get; set; }
        public DbSet<RolePermission> RolePrivileges { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AssignedUserAppRole> AssignedUserApps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>()
                .HasMany(a => a.Environments)
                .WithOne(e => e.App)
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
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Fix for multiple cascade paths

            // Ensure that the email is unique.
            modelBuilder.Entity<Models.User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure that the role name is unique.
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.RolePrivileges)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasMany(p => p.RolePrivileges)
                .WithOne()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure that the privilege name is unique.
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Configure the many-to-many relationship between the role and the privilege.
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePrivileges)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the many-to-many relationship between the privilege and the role.
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePrivileges)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedUserAppRole>()
                .HasKey(aua => new { aua.UserId, aua.AppId, aua.AppRoleId });

            // Configure the one-to-many relationship between the application and the assigned user application.
            modelBuilder.Entity<AssignedUserAppRole>()
                .HasOne(aua => aua.App)
                .WithMany(a => a.AssignedApps)
                .HasForeignKey(aua => aua.AppId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the one-to-many relationship between the user and the assigned user application.
            modelBuilder.Entity<AssignedUserAppRole>()
                .HasOne(aua => aua.User)
                .WithMany(u => u.AssignedApps)
                .HasForeignKey(aua => aua.UserId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
