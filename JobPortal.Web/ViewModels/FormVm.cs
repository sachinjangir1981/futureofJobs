using JobPortal.Domain.Models;

namespace JobPortal.Web.ViewModels
{
    public class FormVm
    {
        public int FormCategoryId { get; set; }
        public List<FormSection> Sections { get; set; } = new();
        // Pre-filled answers if editing:
        public Dictionary<int, string> AnswersText { get; set; } = new(); // QuestionId -> AnswerText (or joined options)
        public Dictionary<int, string> AnswersFile { get; set; } = new(); // QuestionId -> FilePath

        public List<QuestionRating> Ratings { get; set; } = new();

        public double AvgRating { get; set; } = 0;

        public List<Dictionary<string, object>> NewRatings { get; set; } = new(); //  Dynamic Ratings

        public int ProfileId { get; set; } = 0;
    }


    public class FormVmVisitor
    {
        public int FormCategoryId { get; set; }
        public List<FormSection> Sections { get; set; } = new();
        // Pre-filled answers if editing:
        public Dictionary<int, string> AnswersText { get; set; } = new(); // QuestionId -> AnswerText (or joined options)
        public Dictionary<int, string> AnswersFile { get; set; } = new(); // QuestionId -> FilePath

        public List<QuestionRating> Ratings { get; set; } = new();

        public double AvgRating { get; set; } = 0;

        public List<Dictionary<string, object>> NewRatings { get; set; } = new(); //  Dynamic Ratings

        public int ProfileId { get; set; } = 0;
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string FullName { get; set; } = "";
        public string InstituteName { get; set; } = "";
        public DateOnly PostDate { get; set; }  
    }



    public class RegisterModel
    {
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string Placename { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;

    }
}
