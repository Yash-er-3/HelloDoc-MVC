using DataModels.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.ViewModels
{
    public class StudentDataViewmodel
    {

        public int StudentId;

        public string FirstName;

        public string LastName;

        public string Email;

        public string Age;

        public string Gender;

        public string Course;

        public string CourseId;

        public string Grade;

        public DateOnly DOB;


        public string[] GendersList = new[] { "Male", "Female", "Others" };

        List<Student> studentdatalist = new List<Student>();

    }
}
