using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Identity;

namespace NetApp.Infrastructure.Services;

 public class ApplicationDataSeeder : IDatabaseSeeder
    {
        private readonly RoleManager<NetAppRole> _roleManager;
        private readonly UserManager<NetAppUser> _userManager;
        private readonly NetAppDbContext _db;

        public ApplicationDataSeeder(RoleManager<NetAppRole> roleManager, UserManager<NetAppUser> userManager, NetAppDbContext db)
        {
            _roleManager=roleManager;
            _userManager=userManager;
            _db=db;
        }
        private async Task DefaultRolesAsync()
        {
            var superAdminRole = await _roleManager.FindByNameAsync(DomainConstants.Role.SuperAdmin);
            if (superAdminRole == null)
            {
                await _roleManager.CreateAsync(new NetAppRole(DomainConstants.Role.SuperAdmin, "Super user role"){CreatedBy = "System"});
                superAdminRole = await _roleManager.FindByNameAsync(DomainConstants.Role.SuperAdmin);
            }

            foreach (var permission in PermissionHelper.GetAllPermissions().SelectMany(x => x.Permissions).Select(p => p.Value))
                await _roleManager.AddClaimAsync(superAdminRole!, new Claim(SharedConstants.CustomClaimTypes.Permission, permission));

        }

        private async Task DefaultUsersAsync()
        {
            var defaultUser = new NetAppUser
            {
                UserName = "superadmin",
                Email = "olatechlove@gmail.com",
                CreatedBy = "System",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Active = true
            };

            if (await _userManager.FindByEmailAsync(defaultUser.Email) != null)
                return;

            await _userManager.CreateAsync(defaultUser, "P@ssw0rd");
            await _userManager.AddToRoleAsync(defaultUser, DomainConstants.Role.SuperAdmin);
            await _userManager.AddClaimAsync(defaultUser, new Claim(ClaimTypes.NameIdentifier, defaultUser.Id));
            await _userManager.AddClaimAsync(defaultUser, new Claim("username", defaultUser.UserName));

        }

        
        public void Initialize()
        {
            _db.Database.Migrate();
            DefaultRolesAsync().GetAwaiter().GetResult();
            DefaultUsersAsync().GetAwaiter().GetResult();
        }
    }