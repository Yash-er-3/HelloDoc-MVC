using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HelloDoc.ViewModels
{
    public class family
    {
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Valid Firstname")]
        [Required(ErrorMessage = "Firstname is required")]
        public String FirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter a Valid Lastname")]
        [Required(ErrorMessage = "Last Name is required")]
        public String LastName { get; set; }

        [StringLength(23)]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
        [Required(ErrorMessage = "Plese enter your Phone Number")]

        public String PhoneNumber { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        [Required(ErrorMessage = "Please enter your Email Address")]

        public String Email { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Valid Symptoms")]
        [MaybeNull]
        public String RelationWithPatient { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Valid Symptoms")]
        [MaybeNull]
        public String Symptoms { get; set; }


        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Valid Firstname")]
        [Required(ErrorMessage = "Firstname is required")]

        public String PFirstName { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter a Valid Lastname")]
        [Required(ErrorMessage = "Last Name is required")]

        public String PLastName { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required")]

        public DateOnly PDOB { get; set; }
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        [Required(ErrorMessage = "Please enter your Email Address")]

        public String PEmail { get; set; }
        [StringLength(23)]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Please enter valid phone number")]
        [Required(ErrorMessage = "Plese enter your Phone Number")]

        public String PPhoneNumber { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Enter valid Street")]
        [RegularExpression(@"^(?=.*\S)[a-zA-Z0-9\s.,'-]+$", ErrorMessage = "Enter a valid street address")]

        public String Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Enter valid City")]
        [RegularExpression(@"^(?=.*\S)[a-zA-Z\s.'-]+$", ErrorMessage = "Enter a valid city name")]

        public String City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Enter valid State")]
        [RegularExpression(@"^(?=.*\S)[a-zA-Z\s.'-]+$", ErrorMessage = "Enter a valid State name")]

        public String State { get; set; }


        [Required(ErrorMessage = "Zip Code is required")]
        [StringLength(10, ErrorMessage = "Enter valid Zip Code")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit zip code")]
        public String ZipCode { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Valid Symptoms")]
        [MaybeNull]
        public String Room { get; set; }

        public List<IFormFile> FileName { get; set; }

    }
}
