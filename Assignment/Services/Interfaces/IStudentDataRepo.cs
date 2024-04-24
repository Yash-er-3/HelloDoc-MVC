using DataModels.DataModels;
using DataModels.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Services.Interfaces
{
    public interface IStudentDataRepo
    {
        public List<StudentDataViewmodel> GetStudentData();

        public bool CheckForExist(string Course);


        public bool AddStudentData(string FirstName, string LastName, string Email, string DOB, string Gender, string Grade, string Course);

        public StudentDataViewmodel GetStudentDataForEdit(string id);
        public void deleteStudent(string id);
        public bool EditStudentData(string FirstName, string LastName, string Email, string DOB, string Gender, string Grade, string Course,string id);
    }
}