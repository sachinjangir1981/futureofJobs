using System;

namespace JobPortal.Domain.Models;

public enum ApplicationStatus { Pending, Shortlisted, Rejected, Hired }

public class JobApplication
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job? Job { get; set; }
    public int JobSeekerProfileId { get; set; }
    public JobSeekerProfile? JobSeekerProfile { get; set; }
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
}
