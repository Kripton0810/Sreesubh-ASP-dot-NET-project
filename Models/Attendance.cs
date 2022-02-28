using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SreesubhApi.Models
{
    public class Attendance
    {
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string employeeName { get; set; }
        public string loginDate { get; set; }
        public string loginTime { get; set; }
        public string loginAddress { get; set; }
        public double workTime { get; set; }
        public string logoutDate { get; set; }
        public string logoutTime { get; set; }
        public string logoutAddress { get; set; }
        public string status { get; set; }
    }
    public class SpecificeAttandance
    {
        public int EmployeeId { get; set; }
        public string startDate { get; set; }

        public string endDate { get; set; }
        public string inTime { get; set; }
    }
    public class Response
    {
        public bool status { get; set; }
        public string msg { get; set; }
    }
}