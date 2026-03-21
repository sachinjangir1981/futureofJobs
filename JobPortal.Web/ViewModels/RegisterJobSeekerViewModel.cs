using System.ComponentModel.DataAnnotations;

namespace JobPortal.Web.ViewModels
{
    public class RegisterJobSeekerViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Skills { get; set; }
    }
}
