using NokPortalAPI.Entities;

namespace NokPortalAPI.Repositories
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Initialize permissions if there are none
            if (!context.Permissions.Any())
            {
                // Seed initial permissions
                // Permission Id 1-20 are reserved for system permissions
                var permissions = new Permission[]
                {
                    new Permission { Id = 1, Name = "Create User", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 2, Name = "Read User", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 3, Name = "Update User", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 4, Name = "Delete User", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 21, Name = "Create App", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 22, Name = "Read App", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 23, Name = "Update App", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true },
                    new Permission { Id = 24, Name = "Delete App", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Active = true }
                };
                foreach (var permission in permissions)
                {
                    if (!context.Permissions.Any(p => p.Id == permission.Id))
                    {
                        context.Permissions.Add(permission);
                    }
                }
                context.SaveChanges();
            }

            // Initialize roles if there are none
            if (!context.Roles.Any())
            {
                // Seed initial roles
                var roles = new Role[]
                {
                    new Role
                    {
                        Id = 1,
                        Name = "Root",
                        CreatedAt = DateTime.Now,
                        ModifiedAt = DateTime.Now,
                        Active = true,
                        RolePermissions = new List<RolePermission>
                        {
                            new RolePermission { PermissionId = 1 },
                            new RolePermission { PermissionId = 2 },
                            new RolePermission { PermissionId = 3 },
                            new RolePermission { PermissionId = 4 },
                            new RolePermission { PermissionId = 21 },
                            new RolePermission { PermissionId = 22 },
                            new RolePermission { PermissionId = 23 },
                            new RolePermission { PermissionId = 24 }
                        }
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Admin",
                        CreatedAt = DateTime.Now,
                        ModifiedAt = DateTime.Now,
                        Active = true,
                        RolePermissions = new List<RolePermission>
                        {
                            new RolePermission { PermissionId = 1 },
                            new RolePermission { PermissionId = 2 },
                            new RolePermission { PermissionId = 3 },
                            new RolePermission { PermissionId = 4 },
                            new RolePermission { PermissionId = 21 },
                            new RolePermission { PermissionId = 22 },
                            new RolePermission { PermissionId = 23 },
                            new RolePermission { PermissionId = 24 }
                        }
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "EndUser",
                        CreatedAt = DateTime.Now,
                        ModifiedAt = DateTime.Now,
                        Active = true,
                        RolePermissions = new List<RolePermission>()
                    }
                };
                foreach (var role in roles)
                {
                    // Ensure that permissions are not duplicated
                    foreach (var rolePermission in role.RolePermissions)
                    {
                        var existingPermission = context.Permissions.Find(rolePermission.PermissionId);
                        if (existingPermission != null)
                        {
                            rolePermission.Permission = existingPermission;
                        }
                    }

                    if (!context.Roles.Any(r => r.Id == role.Id))
                    {
                        context.Roles.Add(role);
                    }
                }
                context.SaveChanges();
            }

            // Check if there are any users already in the database
            if (!context.Users.Any())
            {
                // Seed initial users
                var users = new User[]
                {
                    new User
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "supot.suk@nokair.com",
                        CreatedAt = DateTime.Now,
                        ModifiedAt = DateTime.Now,
                        Active = true,
                        UserRoles = new List<UserRole>
                        {
                            new UserRole { RoleId = 1 }
                        }
                    },
                    new User
                    {
                        FirstName = "Regular",
                        LastName = "User",
                        Email = "user@nokair.com",
                        CreatedAt = DateTime.Now,
                        ModifiedAt = DateTime.Now,
                        Active = true,
                        UserRoles = new List<UserRole>
                        {
                            new UserRole { RoleId = 3 }
                        }
                    }
                };

                foreach (var user in users)
                {
                    // Ensure that the role is not duplicated
                    foreach (var userRole in user.UserRoles)
                    {
                        var existingRole = context.Roles.Find(userRole.RoleId);
                        if (existingRole != null)
                        {
                            userRole.Role = existingRole;
                        }
                    }

                    if (!context.Users.Any(u => u.Email == user.Email))
                    {
                        context.Users.Add(user);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
