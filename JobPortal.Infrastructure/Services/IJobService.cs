using JobPortal.Domain.Models;
using System.Collections.Generic;

namespace JobPortal.Infrastructure.Services;

public interface IJobService
{
    IEnumerable<Job> Search(string q, int page = 1, int pageSize = 20);
    Job? GetById(int id);
    Job? GetBySlug(string slug);
    void Create(Job job);
    void Update(Job job);
    IEnumerable<Job> GetByCompany(Guid companyId);
    string GenerateSlug(string title);
}
