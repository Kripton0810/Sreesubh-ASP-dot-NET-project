using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SreesubhApi.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public string holidayName { get; set; }
        public string holidayDate { get; set; }
    }
    public class Selectors
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}