 
namespace JobPortal.Web.Models
{
    public class DynamicForm
    {
        public int PKID { get; set; }
        public Guid FormGuid { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; } = 1;
        public int CategoryId { get; set; }
        public Boolean IsActive { get; set; } = true;
        public string ImageUrlLeft { get; set; }
        public string BGUrl { get; set; }
        public string CommonCssinStr { get; set; }
        public CommonCSS CommonCSS { get; set; } = new CommonCSS();

        //public SiteUser User = new SiteUser();
        public List<DynamicFormSection> Sections { get; set; } = new List<DynamicFormSection>();

        public DynamicFormSection CurrentSection { get; set; } = new DynamicFormSection();

        public int SectionId { get; set; }
    }

    public class DynamicFormSection
    {
        public int PKID { get; set; }
        public int FormId { get; set; }
        public string Section { get; set; }
        public string Detail { get; set; }
        public string Footer { get; set; }
        public int Screen { get; set; }
        public int DisplayOrder { get; set; } = 1;
        public Boolean IsActive { get; set; } = true;

        public List<DQuestion> Questions { get; set; } = new List<DQuestion>();
        public DQuestion CurrentQuestion { get; set; } = new DQuestion();
    }


    public class DQuestion
    {
        public int PKID { get; set; }
        public int SectionId { get; set; }

        public int Step { get; set; } = 1;
        public int QuestionType { get; set; } = 1;
        public string Question { get; set; }
        public string Answer { get; set; }
        public decimal Marks { get; set; }
        public int DisplayOrder { get; set; } = 1;
        public Boolean IsActive { get; set; } = true;
        public List<DQuestionAnswer> Answers { get; set; } = new List<DQuestionAnswer>();

        public DQuestionAnswer CurrentAnswer { get; set; } = new DQuestionAnswer();
    }

    public class DQuestionAnswer
    {
        public int PKID { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public string Details { get; set; }
        public decimal Marks { get; set; }
        public int DisplayOrder { get; set; } = 1;
        public int RMin { get; set; }
        public int RMax { get; set; }
        public int Rincrement { get; set; }
        public Boolean IsActive { get; set; } = true;
    }

    public class DAnswerRecord
    {

        public int PKID { get; set; }
        public int SectionId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Details { get; set; }
        public decimal QuestionMarks { get; set; }
        public decimal Marks { get; set; }

        public int QuestionType { get; set; }
    }


    public class DFormCategory
    {
        public int PKID { get; set; }
        public string CategoryName { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum QuestionType
    {
        Radio = 1,
        Checkbox = 2,
        Range = 3,
        Text = 4,
        FileUpload = 5
        //Number = 5,
        //Date = 6,
        //Time = 7,
        //Signature = 9
    }


    public class CommonCSS
    {

        public string SurveyFormatHeaderBG { get; set; } = "#ffffff";
        public string SurveyFormatHeadDetailsBG { get; set; } = "#ffffff";
        public string SurveyFormatFooterBG { get; set; } = "#ffffff";
        public string SurveyFormatSectionBG { get; set; } = "#ffffff";
        public string SurveyFormatSectionDetailsBG { get; set; } = "#ffffff";
        public string SurveyFormatSectionFooterBG { get; set; } = "#ffffff";
        public string QuestionBG { get; set; } = "#ffffff";
        public string AnswerBG { get; set; } = "#ffffff";
    }

    //public class SiteUser
    //{
    //    public int PKID { get; set; }


    //    public string Name { get; set; }
    //    [Required]
    //    public string MobileNo { get; set; }
    //    [Required(ErrorMessage = "Email is required")]
    //    [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
    //    public string EmailId { get; set; }

    //    [Required(ErrorMessage = "Password is required")]
    //    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    //    [DataType(DataType.Password)]
    //    public string UserPassword { get; set; }



    //    public bool IsBootStrap { get; set; }
    //}

    public class UserFormSubmitted
    {
        public int UserId { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string SessionId { get; set; }

    }
}
