using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortal.Domain.Models
{
    public enum QuestionType
    {
        Text = 1,
        SingleChoice = 2,
        MultipleChoice = 3,
        FileUpload = 4,
        Multiline =5
    }

    public class FormTypeCategory
    {
        public int Id { get; set; }
       
        public string FormCategory { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public bool IsReadOnly { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public ICollection<FormSection> Sections { get; set; } = new List<FormSection>();
        public string Details { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string BgImageUrl { get; set; } = string.Empty;
        public string FooterNote { get; set; } = string.Empty;

        public int ForRole { get; set; } // 1=JobSeeker, 2=Employer

        public bool IsPublic { get; set; } = false;

       
    }

    public class FormSection
    {
       
        public int Id { get; set; }
        public int FormTypeCategoryId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;
        

        public bool IsActive { get; set; } = true;

        public ICollection<FormQuestion> Questions { get; set; } = new List<FormQuestion>();
    }

    public class FormQuestion
    {
        public int Id { get; set; }

        public int SectionId { get; set; }
        public FormSection Section { get; set; } = null!;

 
        public string QuestionText { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; }

        public bool IsRequired { get; set; }

        public bool IsRating { get; set; }

        public bool IsCompare { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public ICollection<FormOption> Options { get; set; } = new List<FormOption>();

        public bool IsFilter { get; set; } = false;

        public decimal Weightage { get; set; } = 0;

        public int Marks { get; set; } = 0;

        public string Explanation { get; set; } = "";

        public ICollection<QuestionRating> QuestionRatings { get; set; } = new List<QuestionRating>();
    }


    public class FormOption
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public FormQuestion Question { get; set; } = null!;

        
        public string OptionText { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public decimal Weightage { get; set; } = 0;

        public int Marks { get; set; } = 0;

        public string Explanation { get; set; } = "";
    }

    public class FormAnswer
    {
        public int Id { get; set; }

      
        public string UserId { get; set; } = string.Empty; // FK to AspNetUsers

        public int QuestionId { get; set; }
        public FormQuestion Question { get; set; } = null!;

        // For Text answers OR joined choices (e.g., "3|5|6")
        public string? AnswerText { get; set; }

       
        public string? FilePath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public bool IsDraft { get; set; }   // NEW
        public int AnswerId { get; set; }

        public int ProfileId { get; set; }

        public int FormSessionEntryId { get; set; }
    }


    public class ProfileRating
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // Job Seeker
        public int Score { get; set; }       // 0–10
        public string RatedBy { get; set; }  // Admin UserId
        public DateTime RatedAt { get; set; }

        


    }


    public class QuestionRating
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // Job seeker
        public int QuestionId { get; set; }  // Link to FormQuestion
        public int Score { get; set; }       // 0–10

       
        public string RatedBy { get; set; }  // "Self", "Admin", "Employer"
        public DateTime RatedAt { get; set; }

        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string RatingByName { get; set; }
        [NotMapped]
        public int AvgScore { get; set; }       // 0–10
        public FormQuestion Question { get; set; }

        public int ProfileId { get; set; }
    }

    public enum CustomRoles
    {
        JobSeeker = 1,
        Employer = 2,
        Academics = 3,
        Normal = 4
    }
}
