using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Prep.Models.DBModels
{
    public class Staffs
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public int Store_ID { get; set; }
        public int Manager_ID { get; set; }

        public Staffs()
        {

        }
    }
}