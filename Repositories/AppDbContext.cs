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
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Privileges { get; set; }
        public DbSet<RolePermission> RolePrivileges { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserAppRoleAssignment> UserAppRoleAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure that the app name is unique.
            modelBuilder.Entity<App>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Fix for multiple cascade paths

            // Ensure that the email is unique.
            modelBuilder.Entity<User>()
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
                .HasMany(r => r.RolePermissions)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasMany(p => p.RolePermissions)
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
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the many-to-many relationship between the privilege and the role.
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAppRoleAssignment>()
                .HasKey(aua => new { aua.UserId, aua.AppId, aua.RoleId });

            // Configure the one-to-many relationship between the application and the assigned user application.
            modelBuilder.Entity<UserAppRoleAssignment>()
                .HasOne(aua => aua.App)
                .WithMany(a => a.AssignedApps)
                .HasForeignKey(aua => aua.AppId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the one-to-many relationship between the user and the assigned user application.
            modelBuilder.Entity<UserAppRoleAssignment>()
                .HasOne(aua => aua.User)
                .WithMany(u => u.AssignedApps)
                .HasForeignKey(aua => aua.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
