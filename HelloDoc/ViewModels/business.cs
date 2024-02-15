using System.ComponentModel.DataAnnotations;

namespace HelloDoc.ViewModels
{
    public class business
    {
        [Required]
        public String FirstName { get; set; }
        [Required]
        public String LastName { get; set; }
        [Required]

        public String PhoneNumber { get; set; }

        public String Email { get; set; }

        public String Business { get; set; }

        public string CaseNumber { get; set; }

        public String Symptoms { get; set; }

        [Required]
        public String PFirstName { get; set; }
        [Required]
        public String PLastName { get; set; }

        public DateOnly PDOB { get; set; }
        [Required]

        public String PEmail { get; set; }
        [Required]

        public String PPhoneNumber { get; set; }

        public String Street { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String ZipCode { get; set; }

        public String Room { get; set; }
      
    }
}
