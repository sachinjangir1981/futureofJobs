using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Domain.Models
{
    public class SkillCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }

    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? SkillCategoryId { get; set; }
        public SkillCategory? SkillCategory { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();
    }

    public class JobSkill
    {
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }

    public class JobSeekerSkill
    {
        public int JobSeekerProfileId { get; set; }
        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;
        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }


}
