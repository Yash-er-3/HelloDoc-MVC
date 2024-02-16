using Microsoft.AspNetCore.Http;

namespace HelloDoc.ViewModels
{
    public class family
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String PhoneNumber { get; set; }

        public String Email { get; set; }

        public String RelationWithPatient { get; set; }

        public String Symptoms { get; set; }

        public String PFirstName { get; set; }

        public String PLastName { get; set; }

        public DateOnly PDOB { get; set; }

        public String PEmail { get; set; }

        public String PPhoneNumber { get; set; }


        public String Street { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String ZipCode { get; set; }

        public String Room { get; set; }

        public List<IFormFile> FileName { get; set; }

    }
}
