using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SreesubhApi.Models;
using System.Data.SqlClient;

namespace SreesubhApi.Controllers
{
    public class LeaveController : ApiController
    {
        const int NOT_VIEWED = 0;
        const int APROVED = 1;
        const int DECLINE = 2;
        const int CANCELLED = 3;
        const int TOTAL_LEAVE = 168;
        const int FULL_DAY = 1;
        const int FIRST_SHIFT = 2;
        const int LAST_SHIFT = 3;

        SqlConnection con;
        [HttpGet]
        public IHttpActionResult getAllReport()
        {
            string q = "SELECT * FROM Leave";
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<LeaveManagement> list = new List<LeaveManagement>();
            while (dr.Read())
            {
                LeaveManagement obj = new LeaveManagement();
                obj.leaveId = Convert.ToInt32(dr["Id"]);
                obj.employeeId = Convert.ToInt32(dr["EmployeeId"]);
                obj.employeeName = dr["EmployeeName"].ToString();
                obj.startDate = dr["StartDate"].ToString();
                obj.endDate = dr["EndDate"].ToString();
                obj.description = dr["description"].ToString();
                obj.leaveType = Convert.ToInt32(dr["leaveType"]);
                obj.leaveHour = Convert.ToInt32(dr["leaveHour"]);
                obj.status = Convert.ToInt32(dr["Status"]);
                list.Add(obj);
            }
            con.Close();
            return Ok(list);
        }
        [HttpGet]
        public IHttpActionResult getAllReportNotViewed()
        {
            string q = "SELECT * FROM Leave WHERE Status = "+NOT_VIEWED;
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<LeaveManagement> list = new List<LeaveManagement>();
            while (dr.Read())
            {
                LeaveManagement obj = new LeaveManagement();
                obj.leaveId = Convert.ToInt32(dr["Id"]);
                obj.employeeId = Convert.ToInt32(dr["EmployeeId"]);
                obj.employeeName = dr["EmployeeName"].ToString();
                obj.startDate = dr["StartDate"].ToString();
                obj.endDate = dr["EndDate"].ToString();
                obj.description = dr["description"].ToString();
                obj.leaveType = Convert.ToInt32(dr["leaveType"]);
                obj.leaveHour = Convert.ToInt32(dr["leaveHour"]);
                obj.status = Convert.ToInt32(dr["Status"]);
                list.Add(obj);
            }
            con.Close();
            return Ok(list);
        }
        [HttpGet]
        public IHttpActionResult getAllReportViewed()
        {
            string q = "SELECT * FROM Leave WHERE Status  !=" + NOT_VIEWED;
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<LeaveManagement> list = new List<LeaveManagement>();
            while (dr.Read())
            {
                LeaveManagement obj = new LeaveManagement();
                obj.leaveId = Convert.ToInt32(dr["Id"]);
                obj.employeeId = Convert.ToInt32(dr["EmployeeId"]);
                obj.employeeName = dr["EmployeeName"].ToString();
                obj.startDate = dr["StartDate"].ToString();
                obj.endDate = dr["EndDate"].ToString();
                obj.description = dr["description"].ToString();
                obj.leaveType = Convert.ToInt32(dr["leaveType"]);
                obj.leaveHour = Convert.ToInt32(dr["leaveHour"]);
                obj.status = Convert.ToInt32(dr["Status"]);
                list.Add(obj);
            }
            con.Close();
            return Ok(list);
        }
        [HttpPost]
        public IHttpActionResult applyForLeave(LeaveManagement data)
        {
            con = new Connection().makeConnection();
            int myReaminLeave = 0;
            string q = "select SUM(leaveHour) as total from leave where EmployeeId = " + data.employeeId+" and Status = '"+APROVED+"'";
            SqlCommand sqlCommand = new SqlCommand(q, con);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if(reader.Read())   
            {

                if (!(reader["total"] is DBNull))
                {
                    myReaminLeave = Convert.ToInt32(reader["total"]);
                }
                else
                {
                    myReaminLeave = 0;
                }

            }
            con.Close();

            if(TOTAL_LEAVE-myReaminLeave!=0)
            {
                con = new Connection().makeConnection();
                q = "SELECT * FROM Employee WHERE EmployeeId = " + data.employeeId;
                SqlCommand cmd = new SqlCommand(q, con);
                reader = cmd.ExecuteReader();
                q = "select * from Holiday where HolidayDate BETWEEN '" + data.startDate + "' AND '" + data.endDate + "'";
                SqlConnection con1 = new Connection().makeConnection();
                SqlCommand cmd1 = new SqlCommand(q, con1);
                SqlDataReader red = cmd1.ExecuteReader();
                int holi=0;
                while(red.Read())
                {
                    Holiday holiday = new Holiday();
                    holiday.holidayDate = red["HolidayDate"].ToString();
                    int k = TotalSundays(Convert.ToDateTime(holiday.holidayDate), Convert.ToDateTime(holiday.holidayDate));
                    if(k!=1)
                    {
                        holi++;
                    }
                }
                con1.Close();
                if (reader.Read())
                {
                    data.employeeId = Convert.ToInt32(reader["EmployeeId"]);
                    data.employeeName = reader["FirstName"].ToString() + reader["MiddleName"].ToString() + reader["LastName"].ToString();
                    data.status = NOT_VIEWED;
                    DateTime start = Convert.ToDateTime(data.startDate);
                    DateTime end = Convert.ToDateTime(data.endDate);
                    int TotalDays = Convert.ToInt32((end - start).TotalDays)+1;
                    int sundays = TotalSundays(end, start);
                    TotalDays = TotalDays - sundays- holi;
                    data.leaveHour = TotalDays;//reamin to cut holidays
                    con = new Connection().makeConnection();
                    q = "INSERT INTO Leave (EmployeeId,EmployeeName,StartDate,EndDate,description,leaveType,leaveHour,Status) VALUES "+
                            "('"+ data.employeeId + "','" + data.employeeName + "','" + data.startDate + "','" + data.endDate + "','" + data.description + "','" + data.leaveType + "','" + data.leaveHour + "','" + data.status + "')";
                    SqlCommand insert = new SqlCommand(q,con);
                    insert.ExecuteNonQuery();
                    con.Close();

                    return Ok(data);
                    
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpPut]
        public IHttpActionResult Accept(int id)
        {
            string q = "UPDATE Leave SET Status = '"+APROVED+"' WHERE Id = '"+id+"'";
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q,con);
            cmd.ExecuteNonQuery();
            Response res = new Response();
            con.Close();
            res.msg = "Leave Accepted!!!";
            res.status = true;
            return Ok(res);
        }
        [HttpPut]
        public IHttpActionResult Decline(int id)
        {
            string q = "UPDATE Leave SET Status = '" + DECLINE + "' WHERE Id = '" + id + "'";
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.ExecuteNonQuery();
            Response res = new Response();
            con.Close();
            res.msg = "Leave Declined!!!";
            res.status = true;
            return Ok(res);
        }
        [HttpPut]
        public IHttpActionResult Cancelled(int id)
        {
            string q = "SELECT * FROM Leave WHERE Id = '" + id + "'";
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader reader = cmd.ExecuteReader();
            string date = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            DateTime datenow = Convert.ToDateTime(date);
            if (reader.HasRows)
            {
                reader.Read();
                LeaveManagement leave = new LeaveManagement();
                leave.leaveId = Convert.ToInt32(reader["Id"]);
                leave.employeeId = Convert.ToInt32(reader["EmployeeId"]);
                leave.employeeName = reader["EmployeeName"].ToString();
                leave.startDate = reader["StartDate"].ToString();
                leave.endDate = reader["EndDate"].ToString();
                DateTime leaveStart = Convert.ToDateTime(leave.startDate);
                int remain =Convert.ToInt32((leaveStart - datenow).TotalDays)+1;
                con.Close();
                if (remain>0)
                {
                    q = "UPDATE Leave SET Status = '" + CANCELLED + "' WHERE Id = '" + leave.leaveId + "'";
                    con = new Connection().makeConnection();
                    SqlCommand cmd1 = new SqlCommand(q, con);
                    cmd1.ExecuteNonQuery();
                    Response res = new Response();
                    con.Close();
                    res.msg = "Leave Canceled!!! ";
                    res.status = true;
                    return Ok(res);
                }
                else
                {

                    Response res = new Response();
                    res.msg = "Leave not Canceled!!! ";
                    res.status = false;
                    return Ok(res);
                }

            }
            return BadRequest();
        }
        public int TotalSundays(DateTime end,DateTime start)
        {
            TimeSpan diff = end - start;
            int days = diff.Days;
            int sundays = 0;
            for (var i = 0; i <= days; i++)
            {
                var testDate = start.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        sundays++;
                        break;
                }
            }
            return sundays;
        }
    }
}
