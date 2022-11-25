using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Exam_Prep.Models;

namespace Exam_Prep.Controllers
{
    public class Ajax_JqueryController : Controller
    {
        public BikeStoresEntities db = new BikeStoresEntities();
        //Json 

        public JsonResult table()
        {
            var coporateRelationship = db.customers.GroupBy(c => c.customer_id).Select(r => new
            {
                customer = r.Select(c => c.first_name).FirstOrDefault() + r.Select(c => c.last_name).FirstOrDefault(),
                employee = r.Select(e => e.Employee.FirstName).FirstOrDefault() + r.Select(e => e.Employee.LastName).FirstOrDefault(),
                ReportsTo = r.Select(e => e.Employee.Email).FirstOrDefault()
            }).ToList();



            return new JsonResult { Data = new { relationship = coporateRelationship }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}