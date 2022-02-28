using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using SreesubhApi.Models;
using System.Data.SqlClient;

namespace SreesubhApi.Controllers
{
    public class Connection
    {
        string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["cnnstr"].ConnectionString;
        public SqlConnection makeConnection()
        {
            SqlConnection con = new SqlConnection(cnnStr);
            con.Open();
            return con;
        }
        public void closeConnection(SqlConnection con)
        {
            con.Close();
        }
    }
    
    public class AttendanceController : ApiController
    {
        SqlConnection con = new Connection().makeConnection();
        [HttpPost]
        public IHttpActionResult loginreport()
        {
            string q = "SELECT * FROM Attandance";
            SqlCommand cmd = new SqlCommand(q,con);
            SqlDataReader reader = cmd.ExecuteReader();
            if(reader.HasRows)
            {
                Response resp = new Response();
                resp.msg = "all checked done";
                resp.status = true;
                return Ok(resp);
            }
            else
            {
                Response resp = new Response();
                resp.msg = "data missing";
                resp.status = false;
                return NotFound();

            }
        }

        [HttpPost]
        public IHttpActionResult newAttandance(Attendance data)
        {
            SqlConnection con = new Connection().makeConnection();
            string query = "SELECT * FROM Employee WHERE EmployeeId = " + data.EmployeeId;
            SqlCommand cmd = new SqlCommand(query,con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            Employee emp = new Employee();
            if (dr.HasRows)
            {
                emp.EmployeeId =Convert.ToInt32(dr["EmployeeId"]);
                emp.FirstName = dr["FirstName"].ToString();
                emp.MiddleName = dr["MiddleName"].ToString();
                emp.LastName = dr["LastName"].ToString();
                emp.UserId = dr["UserId"].ToString();
                emp.Role = dr["Role"].ToString();

            }
            try
            {
                int id = emp.EmployeeId;
                string name = emp.FirstName + emp.MiddleName + emp.LastName;
                string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                string date = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                Attendance obj = new Attendance();
                obj.loginTime = time;
                obj.loginDate = date;
                obj.status = "P";
                obj.loginAddress = data.loginAddress;
                obj.employeeName = name;
                obj.logoutDate = null;
                obj.logoutTime = null;
                obj.logoutAddress = null;
                obj.EmployeeId = id;
                con = new Connection().makeConnection();
                query = "INSERT INTO Attendance (EmployeeId, Name, Status, LoginDate, LoginTime, LoginAddress, LogoutDate, LogoutTime, LogoutAddress) VALUES ("+id+",'"+name+"', 'P' ,'"+date+"','"+time+"','"+obj.loginAddress+"', null, null, null)";
                SqlCommand cm = new SqlCommand(query,con);
                cm.ExecuteNonQuery();
                Response response = new Response();
                response.msg = "Attandance Succefully";
                response.status = true;
                con.Close();
                return Ok(response);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("msg", e.ToString());
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public IHttpActionResult makeLogout(Attendance data)
        {
            string date;
            string time;
            string address;
            bool update = false;
            if (data.logoutDate == null)
            {
                time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                date = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                address = data.logoutAddress;
            }
            else
            {
                date = data.logoutDate;
                time = data.logoutTime;
                address = "Logout done in office";
                update = true;
            }
            SqlConnection con = new Connection().makeConnection();
            string query = "SELECT * FROM Attendance WHERE EmployeeId = " + data.EmployeeId + " and LoginDate = '" + date + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            Attendance att = new Attendance();
            if (dr.HasRows)
            {
                att.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                att.loginDate = dr["LoginDate"].ToString();
                att.loginTime = dr["LoginTime"].ToString();
                att.logoutTime = dr["LogoutTime"].ToString();
                att.logoutDate = dr["LogoutDate"].ToString();
                att.logoutAddress = data.logoutAddress;
                int id = Convert.ToInt32(dr["Id"]);
                if (att.loginDate != "")
                {
                    if (att.logoutDate == ""||update)
                    {
                        string q = "UPDATE Attendance SET LogoutDate = '" + date + "', LogoutTime = '" + time + "', LogoutAddress = '" + address  + "' WHERE Id = " + id;
                        con = new Connection().makeConnection();
                        SqlCommand cm = new SqlCommand(q, con);
                        cm.ExecuteNonQuery();
                        Response response = new Response();
                        response.msg = "Logout Succefully done";
                        response.status = true;
                        con.Close();
                        return Ok(response);
                    }
                    else
                    {
                        ModelState.AddModelError("msg", "user already logout");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    ModelState.AddModelError("msg", "user not login...");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                ModelState.AddModelError("msg", "user not login...");
                return BadRequest(ModelState);
            }
        }
        [HttpPut]
        public IHttpActionResult makeLogin(Attendance data)
        {
            string time = data.loginTime;
            string date = data.loginDate;
            string status = data.status;
            
            SqlConnection con = new Connection().makeConnection();
            string query = "SELECT * FROM Attendance WHERE EmployeeId = " + data.EmployeeId+ " and LoginDate = '"+date+"'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            Attendance att = new Attendance();
            if (dr.HasRows)
            {
                string q = "UPDATE Attendance SET LoginDate = '" + date + "', LoginTime = '" + time + "', LoginAddress = 'office location updated manually', Status ='"+ status + "' WHERE Id = " +Convert.ToInt32(dr["Id"]);
                con = new Connection().makeConnection();
                SqlCommand cm = new SqlCommand(q, con);
                cm.ExecuteNonQuery();
                Response response = new Response();
                response.msg = "Updated.....";
                response.status = true;
                con.Close();
                return Ok(response);
            }
            else
            {   //make login
                query = "INSERT INTO Attendance (EmployeeId, Name, Status, LoginDate, LoginTime, LoginAddress, LogoutDate, LogoutTime, LogoutAddress) VALUES (" + data.EmployeeId + ",'" + data.employeeName + "', '"+status+"' ,'" + date + "','" + time + "','office location made manually', null, null, null)";
                //ModelState.AddModelError("msg", "user not login...");
                con = new Connection().makeConnection();
                SqlCommand cm = new SqlCommand(query, con);
                cm.ExecuteNonQuery();
                Response response = new Response();
                response.msg = "Updated...";
                response.status = true;
                con.Close();
                return Ok(response);
            }
        }
        public void sendMail(string Body,string Subject,string to)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            string fromEmail = System.Configuration.ConfigurationManager.ConnectionStrings["from_email"].ConnectionString;
            string fromPassword = System.Configuration.ConfigurationManager.ConnectionStrings["from_password"].ConnectionString;

            mail.From = new MailAddress("bookbin0810@gmail.com");
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(fromEmail, fromPassword); // Enter seders User name and password       
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        [HttpPost]
        public IHttpActionResult specificAttandance(SpecificeAttandance data)
        {
            if (data != null)
            {
                string time = "23:00:00";


                if (data.inTime==null)
                {
                    time = "23:00:00";
                }
                else
                {
                    time = data.inTime;
                }
                string q = "SELECT * FROM Attendance WHERE EmployeeId = " + data.EmployeeId + " and LoginTime <= '" + time + "' and LoginDate  BETWEEN '" + data.startDate + "' AND '" + data.endDate + "' ORDER BY LoginDate DESC";
                con = new Connection().makeConnection();
                SqlCommand cmd = new SqlCommand(q, con);
                SqlDataReader dr = cmd.ExecuteReader();
                List<Attendance> list = new List<Attendance>();
                while (dr.Read())
                {

                    Attendance obj = new Attendance();
                    obj.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                    obj.employeeName = dr["Name"].ToString();
                    obj.status = dr["Status"].ToString();
                    obj.loginDate = Convert.ToDateTime(dr["LoginDate"].ToString()).ToString("yyyy-MM-dd");
                    obj.id = Convert.ToInt32(dr["Id"]);
                    obj.loginAddress = dr["LoginAddress"].ToString();
                    string login = Convert.ToDateTime(dr["LoginTime"].ToString()).ToString("HH:mm:ss");
                    obj.loginTime = login;
                    if (dr["LogoutDate"].ToString() != "")
                    {
                        obj.logoutDate = Convert.ToDateTime(dr["LogoutDate"].ToString()).ToString("yyyy-MM-dd");
                        string logout = Convert.ToDateTime(dr["LogoutTime"].ToString()).ToString("HH:mm:ss");
                        DateTime a = Convert.ToDateTime(login);
                        DateTime b = Convert.ToDateTime(logout);
                        obj.workTime = b.Subtract(a).TotalMinutes;
                        obj.logoutTime = logout;

                    }
                    obj.logoutAddress = dr["LogoutAddress"].ToString();
                    list.Add(obj);
                }
                con.Close();
                return Ok(list);
            }
            else
            {
                Response response = new Response();
                response.msg = "No data found";
                response.status = false;
                return Ok(response);
            }
        }
        [HttpGet]
        public IHttpActionResult specificEmployeeFullAttandance(SpecificeAttandance data)
        {
            string time = data.inTime;
            if (time == null)
            {
                time = "23:00:00";
            }
            string q = "SELECT * FROM Attendance WHERE EmployeeId = " + data.EmployeeId;
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<Attendance> list = new List<Attendance>();
            while (dr.Read())
            {

                Attendance obj = new Attendance();
                obj.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                obj.employeeName = dr["Name"].ToString();
                obj.status = dr["Status"].ToString();
                obj.loginDate = Convert.ToDateTime(dr["LoginDate"].ToString()).ToString("yyyy-MM-dd");
                obj.loginAddress = dr["LoginAddress"].ToString();
                obj.id = Convert.ToInt32(dr["Id"]);
                string login = Convert.ToDateTime(dr["LoginTime"].ToString()).ToString("HH:mm:ss");
                obj.loginTime = login;
                if (dr["LogoutDate"].ToString() != "")
                {
                    obj.logoutDate = Convert.ToDateTime(dr["LogoutDate"].ToString()).ToString("yyyy-MM-dd");
                    string logout = Convert.ToDateTime(dr["LogoutTime"].ToString()).ToString("HH:mm:ss");
                    DateTime a = Convert.ToDateTime(login);
                    DateTime b = Convert.ToDateTime(logout);
                    obj.workTime = b.Subtract(a).TotalMinutes;
                    obj.logoutTime = logout;

                }
                obj.logoutAddress = dr["LogoutAddress"].ToString();
                list.Add(obj);
            }
            con.Close();
            return Ok(list);
        }

        [HttpPost]
        public IHttpActionResult makeReport(SpecificeAttandance data)
        {
            List<TimeManagement> emp = new List<TimeManagement>();
            string employees = "SELECT * FROM Employee";
            con = new Connection().makeConnection();

            int k = TotalSundays(Convert.ToDateTime(data.endDate), Convert.ToDateTime(data.startDate));
            SqlCommand cmd = new SqlCommand(employees, con);
            SqlDataReader dr;
            SqlDataReader dr2 = cmd.ExecuteReader();
            DateTime start = Convert.ToDateTime(data.startDate);
            DateTime end = Convert.ToDateTime(data.endDate);
            int TotalDays = Convert.ToInt32((end - start).TotalDays) + 1;
            while (dr2.Read())
            {
                int total = 0;
                TimeManagement management = new TimeManagement();
                management.Name = dr2["FirstName"].ToString() + " " + dr2["MiddleName"].ToString() + " " + dr2["LastName"].ToString();
                string ontime = "SELECT count(*) as cnt from Employee a Join Attendance b on a.EmployeeId = b.EmployeeId and a.EmployeeId = "+Convert.ToInt32(dr2["EmployeeId"]) + " and b.LoginTime <= DATEADD(minute,2,a.LoginTime) and b.LoginDate >= '"+data.startDate+ "' and b.LoginDate <= '" + data.endDate + "' and b.Status = 'P' ";
                string geacetime = "SELECT count(*) as cnt from Employee a Join Attendance b on a.EmployeeId = b.EmployeeId and a.EmployeeId = " + Convert.ToInt32(dr2["EmployeeId"]) + " and b.LoginTime > DATEADD(minute,2,a.LoginTime) and b.LoginTime <= DATEADD(minute,10,a.LoginTime) and b.LoginDate >= '" + data.startDate + "' and b.LoginDate <= '" + data.endDate + "' and b.Status = 'P'";
                string latetime = "SELECT count(*) as cnt from Employee a Join Attendance b on a.EmployeeId = b.EmployeeId and a.EmployeeId = " + Convert.ToInt32(dr2["EmployeeId"]) + " and b.LoginTime > DATEADD(minute,10,a.LoginTime) and b.LoginDate >= '" + data.startDate + "' and b.LoginDate <= '" + data.endDate + "' and b.Status = 'P'";
                con = new Connection().makeConnection();
                cmd = new SqlCommand(ontime, con);
                dr = cmd.ExecuteReader();
                dr.Read();
                management.countOnTime = Convert.ToInt32(dr["cnt"]);
                total += Convert.ToInt32(dr["cnt"]);
                con = new Connection().makeConnection();
                cmd = new SqlCommand(geacetime, con);
                dr = cmd.ExecuteReader();
                dr.Read();
                management.countGraceTime = Convert.ToInt32(dr["cnt"]);
                total += Convert.ToInt32(dr["cnt"]);

                con = new Connection().makeConnection();
                cmd = new SqlCommand(latetime, con);
                dr = cmd.ExecuteReader();
                dr.Read();
                management.countLateTime = Convert.ToInt32(dr["cnt"]);
                total += Convert.ToInt32(dr["cnt"]);
                management.Total = total;


                emp.Add(management);
            }
            string q = "select count(*) as cnt from Holiday where HolidayDate BETWEEN '" + data.startDate + "' AND '" + data.endDate + "'";
            SqlConnection con1 = new Connection().makeConnection();
            SqlCommand cmd1 = new SqlCommand(q, con1);
            SqlDataReader red = cmd1.ExecuteReader();
            red.Read();
            int holi = Convert.ToInt32(red["cnt"]);
            con1.Close();
            con.Close();
            
            string Body = "";//make body
            //make heading
            Body += "<h1>Attendance Report from " + Convert.ToDateTime(data.startDate).ToString("dd-MM-yyyy").ToString() + " to " + Convert.ToDateTime(data.endDate).ToString("dd-MM-yyyy").ToString()+"</h1>";
            //make table 
            //start table
            Body += "<table border='1'>";
            //table header tr->th
            Body += "<tr><th>Employee Name</th><th>Total Present</th><th bgcolor=\"#42f55d\">Total On Time</th><th bgcolor=\"#f2cd11\">Total Grace Time</th><th bgcolor=\"#f21155\">Total Late Time</th></tr>";
            //make rows
            foreach (TimeManagement item in emp)
            {
                Body += "<tr><th>"+item.Name+ "</th><th>" + item.Total + "</th><th bgcolor=\"#42f55d\">" + item.countOnTime + "</th><th bgcolor=\"#f2cd11\">" + item.countGraceTime + "</th><th bgcolor=\"#f21155\">" + item.countLateTime + "</th></tr>";
            }
            //Close Table and give a br
            Body += "</table><br>";
            //Get Total days and Sundays in between
            TotalDays = TotalDays - k - holi;
            Body += "<p>Total <u>Working days " + TotalDays + "</u></p>";
            Body += "<p>Total <i>Sundays "+k+"</i></p>";
            Body += "<p>Total <em>Holidays days " + holi + "</em></p>";
            string Subject = "Attendance Report";
            string adminmail = "subhankar0810@gmail.com";
            sendMail(Body,Subject,adminmail);
            Response res = new Response();
            res.msg = "Mail send to admin successfully";
            res.status = true;
            return Ok(res);
        }
        [HttpGet]
        public IHttpActionResult getAllReport()
        {
            string q = "SELECT * FROM Attendance";
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<Attendance> list = new List<Attendance>();
            while (dr.Read())
            {

                Attendance obj = new Attendance();
                obj.employeeName = dr["Name"].ToString();
                obj.status = dr["Status"].ToString();
                obj.id = Convert.ToInt32(dr["Id"]);
                obj.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                obj.loginDate = Convert.ToDateTime(dr["LoginDate"].ToString()).ToString("yyyy-MM-dd");
                obj.loginAddress = dr["LoginAddress"].ToString();
                string login = Convert.ToDateTime(dr["LoginTime"].ToString()).ToString("HH:mm:ss");
                obj.loginTime = login;
                if (dr["LogoutDate"].ToString() != "")
                {
                    obj.logoutDate = Convert.ToDateTime(dr["LogoutDate"].ToString()).ToString("yyyy-MM-dd");
                    string logout = Convert.ToDateTime(dr["LogoutTime"].ToString()).ToString("HH:mm:ss");
                    DateTime a = Convert.ToDateTime(login);
                    DateTime b = Convert.ToDateTime(logout);
                    obj.workTime = b.Subtract(a).TotalMinutes;
                    obj.logoutTime = logout;

                }
                obj.logoutAddress = dr["LogoutAddress"].ToString();

                list.Add(obj);
            }
            con.Close();
            return Ok(list);
        }
        public int TotalSundays(DateTime end, DateTime start)
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
