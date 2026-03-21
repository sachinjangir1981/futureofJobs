using JobPortal.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace JobPortal.Web.ViewModels
{
    public class RegisterEmployerViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public string Website { get; set; }
    }

    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Job> LatestJobs { get; set; }
        public List<UserLocation> Locations { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
