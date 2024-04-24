using DataModels.DataContext;
using DataModels.DataModels;
using DataModels.ViewModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repository
{
    public class StudentDataRepo : IStudentDataRepo
    {
        private readonly SMSDbContext smsDbContext;

        public StudentDataRepo(SMSDbContext smsDbContext)
        {
            this.smsDbContext = smsDbContext;
        }

        public List<StudentDataViewmodel> GetStudentData()
        {
            List<StudentDataViewmodel> studentlist = new List<StudentDataViewmodel>();

            var studenttotaldata = smsDbContext.Students.ToList();

            foreach (var student in studenttotaldata)
            {
                StudentDataViewmodel modal = new StudentDataViewmodel();

                modal.StudentId = student.Id;
                modal.FirstName = student.FirstName;
                modal.LastName = student.LastName;
                modal.Email = student.Email;
                modal.Age = student.Age;
                modal.Gender = student.Gender;
                modal.Course = student.Course;
                modal.Grade = student.Grade;

                studentlist.Add(modal);
            }


            return studentlist;
        }

        public bool AddStudentData(string FirstName, string LastName, string Email, string DOB, string Gender, string Grade, string Course)
        {
            var courseid = smsDbContext.Courses.FirstOrDefault(x => x.Name.ToLower().Contains(Course.ToLower())).Id;

            Student modal = new Student
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Gender = Gender,
                Course = Course,
                CourseId = courseid,
                Grade = Grade,
                Age = (DateTime.Now.Year - DateTime.Parse(DOB).Year).ToString()
            };

            if (modal != null)
            {
                smsDbContext.Students.Add(modal);
                smsDbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public bool CheckForExist(string Course)
        {
            var data = smsDbContext.Courses.Where(x => x.Name.ToLower() == Course.ToLower());

            if (data.Any())
            {
                return true;
            }

            Course c = new Course
            {
                Name = Course
            };

            smsDbContext.Courses.Add(c);
            smsDbContext.SaveChanges();
            return false;
        }

        public StudentDataViewmodel GetStudentDataForEdit(string id)
        {
            var rowdata = smsDbContext.Students.FirstOrDefault(x => x.Id == int.Parse(id));

            StudentDataViewmodel modal = new StudentDataViewmodel
            {
                FirstName = rowdata.FirstName,
                LastName = rowdata.LastName,
                Email = rowdata.Email,
                Gender = rowdata.Gender,
                Grade = rowdata.Grade,
                Course = rowdata.Course
            };
            return modal;
        }

        public bool EditStudentData(string FirstName, string LastName, string Email, string DOB, string Gender, string Grade, string Course, string id)
        {

            var data = smsDbContext.Students.FirstOrDefault(x => x.Id == int.Parse(id));
            var courseid = smsDbContext.Courses.FirstOrDefault(x => x.Name.ToLower().Contains(Course.ToLower())).Id;

            data.FirstName = FirstName;
            data.LastName = LastName;
            data.Email = Email;
            data.Gender = Gender;
            data.Course = Course;
            data.CourseId = courseid;
            data.Grade = Grade;
            data.Age = (DateTime.Now.Year - DateTime.Parse(DOB).Year).ToString() != null ? (DateTime.Now.Year - DateTime.Parse(DOB).Year).ToString() : "18";

            if (data != null)
            {
                smsDbContext.Students.Update(data);
                smsDbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public void deleteStudent(string id)
        {
            var rowdata = smsDbContext.Students.FirstOrDefault(x => x.Id == int.Parse(id));

            smsDbContext.Remove(rowdata);
            smsDbContext.SaveChanges();

        }

      

    }
}
