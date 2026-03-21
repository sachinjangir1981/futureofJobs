using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobPortal.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _db;
    public JobService(AppDbContext db) => _db = db;

    public void Create(Job job)
    {
        _db.Jobs.Add(job);
        _db.SaveChanges();
    }

    public Job? GetById(int id) => _db.Jobs.Find(id);

    public Job? GetBySlug(string slug) => _db.Jobs.FirstOrDefault(j => j.Slug == slug && j.Status == JobStatus.Published);

    public IEnumerable<Job> Search(string q, int page = 1, int pageSize = 20)
    {
        var query = _db.Jobs.Where(j => j.Status == JobStatus.Published);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(j => EF.Functions.Like(j.Title, $"%{q}%") || EF.Functions.Like(j.Description, $"%{q}%"));
        return query.OrderByDescending(j => j.PublishedAt).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
    }

    public void Update(Job job)
    {
        job.UpdatedAt = DateTime.UtcNow;
        _db.Jobs.Update(job);
        _db.SaveChanges();
    }

    public IEnumerable<Job> GetByCompany(Guid companyId)
    {
        return _db.Jobs.Where(j => j.CompanyId == companyId).OrderByDescending(j => j.PublishedAt).AsNoTracking().ToList();
    }

    public string GenerateSlug(string title)
    {
        var slug = title.ToLower().Replace(' ', '-');
        var existingSlugs = _db.Jobs.Where(j => j.Slug.StartsWith(slug)).Select(j => j.Slug).ToList();
        if (!existingSlugs.Contains(slug))
            return slug;
        int suffix = 1;
        while (existingSlugs.Contains($"{slug}-{suffix}"))
            suffix++;
        return $"{slug}-{suffix}";
    }
}
