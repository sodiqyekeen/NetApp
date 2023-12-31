using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Entities;
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
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
    }
    private async Task DefaultRolesAsync()
    {
        var superAdminRole = await _db.Roles
        .Include(r => r.RoleClaims.Where(c => c.ClaimType == SharedConstants.CustomClaimTypes.Permission))
        .FirstOrDefaultAsync(r => r.Name == DomainConstants.Role.SuperAdmin); //_roleManager.FindByNameAsync(DomainConstants.Role.SuperAdmin);
        if (superAdminRole == null)
        {
            await _roleManager.CreateAsync(new NetAppRole(DomainConstants.Role.SuperAdmin, "Super user role") { CreatedBy = "System" });
            superAdminRole = await _roleManager.FindByNameAsync(DomainConstants.Role.SuperAdmin);
        }

        foreach (var permission in PermissionHelper.GetAllPermissions().SelectMany(x => x.Permissions).Select(p => p.Value))
        {
            if (superAdminRole!.RoleClaims.Any(x => x.ClaimValue == permission)) continue;
            await _roleManager.AddClaimAsync(superAdminRole!, new Claim(SharedConstants.CustomClaimTypes.Permission, permission));
        }
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
            Active = true,
            FirstName = "Sodiq",
            LastName = "Yekeen"
        };

        if (await _userManager.FindByEmailAsync(defaultUser.Email) != null)
            return;

        await _userManager.CreateAsync(defaultUser, "P@ssw0rd");
        await _userManager.AddToRoleAsync(defaultUser, DomainConstants.Role.SuperAdmin);

    }

    private async Task DefaultEmailTemplatesAsync()
    {
        //Confirm email address email
        var confirmEmailTemplate = await _db.EmailTemplates.FirstOrDefaultAsync(x => x.Name == DomainConstants.EmailTemplate.ConfirmEmail);
        if (confirmEmailTemplate == null)
        {
            // Raw HTML email body using razor template
            var mailCOntent = """
                <html>
                <body>
                <p>Hi @Model.Name,</p>
                    <p>Please click this <a href="@Model.Link" target="_blank" >link</a> to confirm your account.</p>
                    </body>
                    </html>
                """;

            confirmEmailTemplate = new EmailTemplate
            {
                Name = DomainConstants.EmailTemplate.ConfirmEmail,
                Subject = "Confirm your email address",
                Content = mailCOntent,
                CreatedBy = "System"
            };
            await _db.EmailTemplates.AddAsync(confirmEmailTemplate);
        }

        //Reset password email
        var resetPasswordTemplate = await _db.EmailTemplates.FirstOrDefaultAsync(x => x.Name == DomainConstants.EmailTemplate.ResetPassword);
        if (resetPasswordTemplate == null)
        {
            var mailCOntent = """
                <html>
                <body>
                <p>Hi @Model.Name,</p>
                    <p>Kindly use this token <strong>@Model.Token</strong> to reset your password. The token is valid for <strong>5 mintues</strong>.</p>
                    </body>
                    </html>
                """;
            resetPasswordTemplate = new EmailTemplate
            {
                Name = DomainConstants.EmailTemplate.ResetPassword,
                Subject = "Reset your password",
                Content = mailCOntent,
                CreatedBy = "System"
            };
            await _db.EmailTemplates.AddAsync(resetPasswordTemplate);
        }

        await _db.SaveChangesAsync();
    }

    public void Initialize()
    {
        _db.Database.Migrate();
        DefaultRolesAsync().GetAwaiter().GetResult();
        DefaultUsersAsync().GetAwaiter().GetResult();
        DefaultEmailTemplatesAsync().GetAwaiter().GetResult();
    }
}