using NetApp.Domain.Entities;

namespace NetApp.Domain.Repositories;

public interface IEmailTemplateRepository
{
    Task AddTemplateAsync(EmailTemplate emailTemplate);
    Task<EmailTemplate?> GetTemplateByNameAsync(string name);
    Task<EmailTemplate?> GetTemplateByIdAsync(int id);
    void UpdateTemplate(EmailTemplate emailTemplate);
    void DeleteTemplate(EmailTemplate emailTemplate);
}
