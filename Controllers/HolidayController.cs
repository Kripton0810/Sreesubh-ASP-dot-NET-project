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
    public class HolidayController : ApiController
    {
        SqlConnection con;
        
        [HttpGet]
        public IHttpActionResult getHoliday(Selectors data)
        {
            con = new Connection().makeConnection();
            string q = "SELECT * FROM Holiday WHERE HolidayDate BETWEEN '" + data.StartDate + "' AND '" + data.EndDate + "'";
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader reader = cmd.ExecuteReader();
            List<Holiday> list = new List<Holiday>();
            while(reader.Read())
            {
                Holiday obj = new Holiday();
                obj.Id = Convert.ToInt32(reader["Id"]);
                obj.holidayName = reader["HolidayName"].ToString();
                obj.holidayDate = Convert.ToDateTime(reader["HolidayDate"].ToString()).ToString("dd-MM-yyyy");
                list.Add(obj);
            }
            con.Close();
            return Ok(list);
        }
        [HttpPost]
        public IHttpActionResult setHoliday(Holiday data)
        {
            con = new Connection().makeConnection();
            string q = "INSERT INTO Holiday VALUES ('"+data.holidayName+"', '"+data.holidayDate+"')";
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.ExecuteNonQuery();
            Response resp = new Response();
            resp.msg = "Inserted Successfully!!!";
            resp.status = true;
            con.Close();
            return Ok(resp);
        }

        [HttpPut]
        public IHttpActionResult editHoliday(Holiday data)
        {
            string q = "UPDATE Holiday SET HolidayName = '" + data.holidayName + "' , HolidayDate = '" + data.holidayDate + "' WHERE Id = "+data.Id;
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.ExecuteNonQuery();
            Response resp = new Response();
            resp.msg = "Updated Successfully!!!";
            resp.status = true;
            con.Close();
            return Ok(resp);
        }
        [HttpDelete]
        public IHttpActionResult deleteHoliday(int id)
        {
            string q = "DELETE FROM Holiday WHERE Id = "+id;
            con = new Connection().makeConnection();
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.ExecuteNonQuery();
            Response resp = new Response();
            resp.msg = "Delete Successfully!!!";
            resp.status = true;
            con.Close();
            return Ok(resp);
        }
    }
}
