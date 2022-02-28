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
    
    public class EmployeeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult EmployeeReport(string a, string b, string c)
        {
            List<Employee> obj = new List<Employee>();

            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["cnnstr"].ConnectionString;
            SqlConnection con = new SqlConnection(cnnStr);
            con.Open();

            string sql;
            sql = "SELECT * FROM Employee";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Employee item = new Employee();
                item.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                item.EmployeeName = dr["Name"].ToString();
                item.Salary = Convert.ToDecimal(dr["Salary"]);

                obj.Add(item);
            }
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult EmployeeData(string id)
        {
            Employee obj = new Employee();
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["cnnstr"].ConnectionString;
            SqlConnection con = new SqlConnection(cnnStr);
            con.Open();

            string sql;
            sql = "SELECT * FROM Employee WHERE EmployeeId=" + id;
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                obj.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                obj.EmployeeName = dr["Name"].ToString();
                obj.Salary = Convert.ToDecimal(dr["Salary"]);
                dr.Close();
                con.Close();

                return Ok(obj);
            }
            else
            {
                return NotFound();
            }


        }

        [HttpGet]
        public string EmployeeName(string id, string name)
        {
            return "Subhankar";
        }

        

        

        [HttpPost]
        public IHttpActionResult SaveEmployeeData(Employee inp)
        {
            try
            {
                EmployeeResult employeeResult = new EmployeeResult();
                string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["cnnstr"].ConnectionString;
                SqlConnection con = new SqlConnection(cnnStr);
                con.Open();
                string sql;

                sql = "INSERT INTO EMPLOYEE(EmployeeId, Name, Salary) VALUES('" + inp.EmployeeId + "','" + inp.EmployeeName + "','" + inp.Salary + "')";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                con.Close();
                employeeResult.Success = true;
                employeeResult.ErrorMsg = "";

                return Ok(employeeResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
        [HttpGet]
        public IHttpActionResult getEmployee()
        {
            List<Employee> list = new List<Employee>();
            SqlConnection con = new Connection().makeConnection();
            String Q= "SELECT * FROM Employee";
            SqlCommand cmd = new SqlCommand(Q,con);
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                Employee emp = new Employee();
                emp.EmployeeId = Convert.ToInt32(reader["EmployeeId"]);
                emp.EmployeeName = reader["FirstName"].ToString()+" "+ reader["MiddleName"].ToString()+ " "+reader["LastName"].ToString();
                emp.MobileNo = reader["MobileNo"].ToString();
                list.Add(emp);
            }
            return Ok(list);
        }


    }
}
