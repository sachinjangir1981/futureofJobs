using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using System.Linq;

namespace JobPortal.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly AppDbContext _db;
    public ApplicationService(AppDbContext db) => _db = db;

    public void Apply(int jobId, int profileId)
    {
        var exists = _db.JobApplications.Any(a => a.JobId == jobId && a.JobSeekerProfileId == profileId);
        if (exists) return;
        var app = new JobApplication { JobId = jobId, JobSeekerProfileId = profileId };
        _db.JobApplications.Add(app);
        _db.SaveChanges();
    }

    public IEnumerable<JobApplication> GetForJob(int jobId)
        => _db.JobApplications.Where(a => a.JobId == jobId).OrderByDescending(a => a.AppliedOn).ToList();

    public IEnumerable<JobApplication> GetForSeeker(int profileId)
        => _db.JobApplications.Where(a => a.JobSeekerProfileId == profileId).OrderByDescending(a => a.AppliedOn).ToList();

    public JobApplication? GetById(int id) => _db.JobApplications.Find(id);

    public void UpdateStatus(int applicationId, string status)
    {
        var app = _db.JobApplications.Find(applicationId);
        if (app == null) return;
        if (System.Enum.TryParse<JobPortal.Domain.Models.ApplicationStatus>(status, out var s))
            app.Status = s;
        _db.JobApplications.Update(app);
        _db.SaveChanges();
    }
}
