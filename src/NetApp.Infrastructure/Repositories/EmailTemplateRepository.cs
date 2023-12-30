using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Entities;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;

namespace NetApp.Infrastructure.Repositories;

internal class EmailTemplateRepository(NetAppDbContext dbContext) : RepositoryBase<EmailTemplate>(dbContext), IEmailTemplateRepository
{
    public async Task AddTemplateAsync(EmailTemplate emailTemplate) => await AddAsync(emailTemplate);

    public void DeleteTemplate(EmailTemplate emailTemplate) => Delete(emailTemplate);

    public async Task<EmailTemplate?> GetTemplateByIdAsync(int id) => await GetByIdAsync(id);

    public async Task<EmailTemplate?> GetTemplateByNameAsync(string name) =>
        await GetAll().FirstOrDefaultAsync(x => x.Name == name);

    public void UpdateTemplate(EmailTemplate emailTemplate) => Update(emailTemplate);

}
