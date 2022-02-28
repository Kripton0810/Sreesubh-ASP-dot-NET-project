using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SreesubhApi.Models
{
    public class LeaveManagement
    {
        public int leaveId { get; set; }
        public int employeeId { get; set; }
        public string employeeName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string description { get; set; }
        public int leaveType { get; set; }
        // FULL , FL-> FIRST LEAVE, SL-> SECOND LEAVE
        public long leaveHour { get; set; }

        // public string 
        public int status { get; set; }
    }

}