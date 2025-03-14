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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserAppRoleAssignment> UserAppRoleAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure that the app name is unique.
            modelBuilder.Entity<App>()
                .HasIndex(a => a.Name)
                .IsUnique();

            // Ensure that the email is unique.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Ensure that the role name is unique.
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Ensure that the privilege name is unique.
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>()
                .ToTable("User_Role")
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Configure the many-to-many relationship between the role and the user.
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the many-to-many relationship between the user and the role.
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RolePermission entity
            modelBuilder.Entity<RolePermission>()
                .ToTable("Role_Permission")
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

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

            // Configure UserAppRoleAssignment entity
            modelBuilder.Entity<UserAppRoleAssignment>()
                .ToTable("User_App_Role_Assignment")
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

            // Disable identity generation for the Id properties
            modelBuilder.Entity<Permission>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Role>()
                .Property(r => r.Id)
                .ValueGeneratedNever();
        }

    }
}
