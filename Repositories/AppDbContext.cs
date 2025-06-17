using Microsoft.EntityFrameworkCore;
using NokPortalAPI.Entities;
using NokPortalAPI.Enums;

namespace NokPortalAPI.Repositories
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<App> Apps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserAppRoleAssignment> UserAppRoleAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // กำหนดค่า data types สำหรับ DateTime ให้เป็น timestamp with time zone
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp without time zone");
                    }
                }
            }

            ConfigureAppEntity(modelBuilder);
            ConfigureUserEntity(modelBuilder);
            ConfigureRoleEntity(modelBuilder);
            ConfigurePermissionEntity(modelBuilder);
            ConfigureUserRoleEntity(modelBuilder);
            ConfigureRolePermissionEntity(modelBuilder);
            ConfigureUserAppRoleAssignmentEntity(modelBuilder);
        }

        private static void ConfigureAppEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>(entity =>
            {
                entity.ToTable("apps");

                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(a => a.Name).HasColumnName("name").IsRequired().HasMaxLength(150);
                entity.Property(a => a.Header).HasColumnName("header").HasMaxLength(250);
                entity.Property(a => a.Subheader).HasColumnName("sub_header").HasMaxLength(250);
                entity.Property(a => a.EnvironmentType).HasColumnName("environment_type").HasDefaultValue(EnvironmentType.Dev).HasSentinel(EnvironmentType.Dev);
                entity.Property(a => a.ImageUrl).HasColumnName("image_url").HasMaxLength(250);
                entity.Property(a => a.ClientUrl).HasColumnName("client_url").HasMaxLength(250);
                entity.Property(a => a.BackendUrl).HasColumnName("backend_url").HasMaxLength(250);
                entity.Property(a => a.SecretKey).HasColumnName("secret_key").HasMaxLength(200);
                entity.Property(a => a.JwtExpiryHours).HasColumnName("jwt_expiry_hours").IsRequired().HasDefaultValue(8);
                entity.Property(a => a.Remark).HasColumnName("remark").HasMaxLength(500);
                entity.Property(a => a.Active).HasColumnName("active").IsRequired();
                entity.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(a => a.ModifiedAt).HasColumnName("modified_at").IsRequired();

                entity.HasIndex(a => a.Name).IsUnique();
            });
        }

        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(u => u.ObjectId).HasColumnName("object_id").IsRequired().HasMaxLength(150);
                entity.Property(u => u.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
                entity.Property(u => u.JobTitle).HasColumnName("job_title").HasMaxLength(100);
                entity.Property(u => u.Department).HasColumnName("department").HasMaxLength(100);
                entity.Property(u => u.Active).HasColumnName("active").IsRequired();
                entity.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(u => u.ModifiedAt).HasColumnName("modified_at").IsRequired();

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.ObjectId).IsUnique();
            });
        }

        private static void ConfigureRoleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(r => r.Active).HasColumnName("active").IsRequired();
                entity.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(r => r.ModifiedAt).HasColumnName("modified_at").IsRequired();

                entity.HasIndex(r => r.Name).IsUnique();
            });
        }

        private static void ConfigurePermissionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(p => p.Active).HasColumnName("active").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(p => p.ModifiedAt).HasColumnName("modified_at").IsRequired();

                entity.HasIndex(p => p.Name).IsUnique();
            });
        }

        private static void ConfigureUserRoleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_roles");

                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.Property(ur => ur.UserId).HasColumnName("user_id");
                entity.Property(ur => ur.RoleId).HasColumnName("role_id");

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureRolePermissionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions");

                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                entity.Property(rp => rp.RoleId).HasColumnName("role_id");
                entity.Property(rp => rp.PermissionId).HasColumnName("permission_id");

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureUserAppRoleAssignmentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAppRoleAssignment>(entity =>
            {
                entity.ToTable("user_app_role_assignments");

                entity.HasKey(ua => new { ua.UserId, ua.AppId, ua.RoleId });
                entity.Property(ua => ua.UserId).HasColumnName("user_id");
                entity.Property(ua => ua.AppId).HasColumnName("app_id");

                entity.HasOne(ua => ua.User)
                    .WithMany(u => u.UserAppRoleAssignments)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ua => ua.App)
                    .WithMany(a => a.UserAppRoleAssignments)
                    .HasForeignKey(ua => ua.AppId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}