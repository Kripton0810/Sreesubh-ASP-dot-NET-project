using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SreesubhApi.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Salary { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public string DOJ { get; set; }
        public string Photo { get; set; }

    }
    public class TimeManagement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int countOnTime { get; set; }
        public int countGraceTime { get; set; }
        public int countLateTime { get; set; }
        public int Total { get; set; }

    }
    public class EmployeeResult
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
    }
}