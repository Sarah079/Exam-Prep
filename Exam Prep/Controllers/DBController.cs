//this uses the methods in DBModels
//simmiliar to HW05 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Exam_Prep.Models;
using Exam_Prep.Models.DBModels;

namespace Exam_Prep.Controllers
{
    public class DBController : Controller
    {
        //create model for each table in database 
        // GET: DB
        private readonly Dataservice dataservice = Dataservice.GetDataservice();
        //lists
        public static List<Staffs> staffs = new List<Staffs>();

        public ActionResult Index()
        {
            
                dataservice.openConnection();
                List<Staffs> employees = dataservice.GetAllEmployees();
                staffVM employeedetails = new staffVM
                {
                    staff = employees
                };     


            return View(employeedetails);
        }

        //[HttpPost]
        //public ActionResult Index(string searchString, int managerid, int storeid)
        //{
        //    // filter list based on multiple conditionals
        //    //------------Linq Lambda--------------
        //    List<Staffs> staffs = dataservice.GetAllEmployees().Where(x => (searchString != "" ? x.Firstname.ToLower().Contains(searchString.ToLower()) : true)
        //                                                                 && (managerid != 0 ? x.Manager_ID == managerid : true)
        //                                                                 && (storeid != 0 ? x.Store_ID == storeid : true)).ToList();

        //    staffVM bookdetails = new staffVM
        //    {
        //        books = books,
        //        types = types,
        //        authors = authors,
        //        borrows = borrows
        //    };

        //    return View(bookdetails);
        //}
    }
}
//----------------------------ILogger example -------------

//private readonly ILogger<HomeController> _logger;
//private SomeDataService dataService = SomeDataService.getInstance();

//public HomeController(ILogger<HomeController> logger)
//{
//    _logger = logger;
//}

//public IActionResult Index()
//{
//    List<Course> courses = dataService.getAvailableCourses();
//    return View(courses);
//}

//public ActionResult ViewCourse(int courseID)
//{
//    List<MarkedCourseAssignment> markedAssignments = dataService.getAssignmentsOfCourse(courseID);
//    return View(markedAssignments);
//}