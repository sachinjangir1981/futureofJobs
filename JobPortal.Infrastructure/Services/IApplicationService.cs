using JobPortal.Domain.Models;
using System.Collections.Generic;

namespace JobPortal.Infrastructure.Services;

public interface IApplicationService
{
    void Apply(int jobId, int profileId);
    IEnumerable<JobApplication> GetForJob(int jobId);
    IEnumerable<JobApplication> GetForSeeker(int profileId);
    JobApplication? GetById(int id);
    void UpdateStatus(int applicationId, string status);
}
