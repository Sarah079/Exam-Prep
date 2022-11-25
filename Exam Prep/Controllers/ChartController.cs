//uses chartdata model and db 
//We are gettin getting the amount of sales per staff
//Lambda query version and dynamic colour arrays in LinqLambdaVersion() below
// + viewbag example 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using Exam_Prep.Models.DBModels;
using Exam_Prep.Models.ProductsVM;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Exam_Prep.Models;
//Lambda =>

namespace Exam_Prep.Controllers
{
    public class ChartController : Controller
    {
        public BikeStoresEntities db = new BikeStoresEntities();

        [HttpGet]
        public JsonResult Chart()//make Json result
        {

        //selecting all info into a list -- Linq Lambda query (remember lambda vs linq query)
            List<chartdata> employees = db.staffs.Select(x => new chartdata { staff_id = x.staff_id, employee = x.first_name + "" + x.last_name, total_sales = 0 }).ToList(); 
            
        //joins (linq query) --- a linq lambda version of this can be found below
            List<chartdata> orders = (from oi in db.order_items
                                      join o in db.orders on oi.order_id equals o.order_id
                                      join st in db.staffs on o.staff_id equals st.staff_id
                                      group oi by oi.order_id into g //groups orders together for order culculation
        //selecting
                                      select new
                                      {
                                          employee = g.Select(x => x.order.staff.first_name).FirstOrDefault() + g.Select(x => x.order.staff.last_name).FirstOrDefault(),
                                          ordertotal = (int)g.Sum(oi => oi.quantity * oi.list_price), //culculating order total
                                          staff_id = (int)g.Select(st => st.order.staff_id).FirstOrDefault()
                                      })
                                      .ToList()
        //remember to group 
                                      .GroupBy(x => x.employee) //group employees to be used with key
        //add new chartdata object
                                      .Select(x => new chartdata
                                      {
                                          employee = x.Key,
                                          staff_id = x.Select(y => y.staff_id).FirstOrDefault(),
                                          total_sales = (int)x.Sum(y => y.ordertotal)
                                      }).ToList();

        //add employees to the list
            foreach (var emp in employees)
            {
                if (!orders.Select(x => x.employee).Contains(emp.employee))
                {
                    orders.Add(emp);
                }
            }
            return new JsonResult { Data = orders.OrderBy(st => st.staff_id), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //can reuse the above line. Is format for all Json returns 
        }

//---Linq Lambda version--- => (likeley used with EF)
        public JsonResult LinqLambdaVersion()
        {
            List<chartdata> orders = db.order_items.GroupBy(oi => oi.order_id).Select(g => new
            {
                employee = g.Select(x => x.order.staff.first_name).FirstOrDefault() + g.Select(x => x.order.staff.last_name).FirstOrDefault(),
                ordertotal = (int)g.Sum(oi => oi.quantity * oi.list_price), //culculating order total
                staff_id = (int)g.Select(st => st.order.staff_id).FirstOrDefault()
            })
                .ToList().GroupBy(x => x.employee).Select(x => new chartdata
                {
                    employee = x.Key,
                    staff_id = x.Select(y => y.staff_id).FirstOrDefault(),
                    total_sales = (int)x.Sum(y => y.ordertotal)
                }).ToList();


            //can keep array of colours in controller of in ajax  (Dynamically)          
            string[] BackColors = new string[]{"rgba(255, 99, 132, 0.2)","rgba(54, 162, 235, 0.2)","rgba(255, 206, 86, 0.2)",
                                              "rgba(75, 192, 192, 0.2)","rgba(153, 102, 255, 0.2)","rgba(255, 159, 64, 0.2)",
                                              "rgba(121, 194, 50, 0.2)","rgba(150, 127,23, 0.2)","rgba(188, 113, 12, 0.2)","rgba(0, 0, 88, 0.2)" };//Holding ARRAY OF COLORS

            string[] BorderColors = new string[]{"rgba(255, 99, 132, 1)","rgba(54, 162, 235, 1)","rgba(255, 206, 86, 1)",
                                                "rgba(75, 192, 192, 1)","rgba(153, 102, 255, 1)","rgba(255, 159, 64, 1)","rgba(255, 159, 64, 1)", "rgba(255, 159, 64, 1)",
                                                "rgba(255, 159, 64, 1)",
                                                "rgba(255, 159, 64, 1)", };
            //assign colours
            foreach (var emp in orders)
            {
                Random rand = new Random();
                int num = rand.Next(0, 9);
                emp.backgroundColor = BackColors[num];
                emp.borderColor = BorderColors[num];
            }

            return new JsonResult { Data = orders.OrderBy(st => st.staff_id), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //ViewBag.Regions = Regions;
        //ViewBag.BorderColors = bordercolors
    }
}


//SQL:
//select a.employee, sum(a.orderTotal) as Total_Sales  from (
//SELECT CONCAT(st.first_name , '', st.last_name ) as employee,
//       o.order_id,
//       sum(oi.quantity * oi.list_price) as orderTotal
//  FROM[BikeStores].[sales].[order_items] oi
//  Join[BikeStores].[sales].[orders] o on oi.order_id = o.order_id
//  join[BikeStores].[sales].[staffs] st on o.staff_id = st.staff_id
//  group by st.first_name ,st.last_name,oi.quantity,oi.list_price,o.order_id
//  ) a
//  group by a.employee