using System;
using System.Collections.Generic;

namespace JobPortal.Domain.Models;

public enum JobType {
    Remote,
    Office,
    Hybrid
}
public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Internship,
    Temporary
}
public enum JobStatus { Draft = 0, Published = 1, Closed = 2 }

public class Job
{
    public int Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    
    // Type of job (Remote / Office / Hybrid)
    public JobType JobType { get; set; }

    // Employment type (Full-Time / Part-Time / Contract / Internship)
    public EmploymentType EmploymentType { get; set; }

    public int Positions { get; set; } = 1;
    // Location
    public int? StateId { get; set; }
    public State State { get; set; }
    public int? CityId { get; set; }
    public City City { get; set; }
    // Salary
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string SalaryCurrency { get; set; } = "INR"; // or USD, EUR

    // Positions
    public int NumberOfOpenings { get; set; }
    // Experience
    public int MinExperienceYears { get; set; }
    public int? MaxExperienceYears { get; set; }

    // Education Requirement
    public string EducationRequirement { get; set; }

    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? ExpiryAt { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public string UserId { get; set; }   // FK to AspNetUsers
}


public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UserLocation
{
    public int Id { get; set; }
    public string City { get; set; }
}