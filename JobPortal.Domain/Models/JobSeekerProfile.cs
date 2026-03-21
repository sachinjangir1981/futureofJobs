using System;
using System.Collections.Generic;

namespace JobPortal.Domain.Models;
public class JobSeekerProfile
{
    public int Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    
    public int? CurrentStateId { get; set; }
    public int? CurrentCityId { get; set; }
    public State CurrentState { get; set; }
    public City CurrentCity { get; set; }

    // Desired Location
    public int? DesiredStateId { get; set; }
    public int? DesiredCityId { get; set; }
    public State DesiredState { get; set; }
    public City DesiredCity { get; set; }

    public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();


    public DateTime RcdInsTs { get; set; }

    public DateTime? RcdUpdtTs { get; set; }   
    
}


public class AppUser
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string UserType { get; set; } 


}