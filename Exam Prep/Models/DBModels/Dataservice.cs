using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Exam_Prep.Models.DBModels
{
    public class Dataservice
    {
        private SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
        private SqlConnection currConnection;

        private static Dataservice instance;

        public static Dataservice GetDataservice()
        {
            if (instance == null)
            {
                instance = new Dataservice();
            }
            return instance;
        }

        public string getConnectionString()
        {
            stringBuilder["Data Source"] = ".\\SQLEXPRESS";
            stringBuilder["Integrated Security"] = "true";
            stringBuilder["Initial Catalog"] = "BikeStores";
            //or urrConnection = new SqlConnection(@"Data Source=.\SQLEXPRESS; Integrated Security=true; Initial Catalog=BikeStores;");
            return stringBuilder.ToString();
        }
        public void openConnection()
        {
            //bool status = true;
            try
            {
                String conString = getConnectionString();
                currConnection = new SqlConnection(conString);
                currConnection.Open();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                closeConnection();
                //status = false;
            }
        }

        public bool closeConnection()
        {
            if (currConnection != null)
            {
                currConnection.Close();
            }
            return true;
        }
        //remember to always use try and catches when working with manual db integration
        public List<Staffs> GetAllEmployees()
        {
            List<Staffs> employees = new List<Staffs>();

            try
            {
                openConnection();
                SqlCommand command = new SqlCommand("select * from staff", currConnection); //currConnection declared as an empty carrier for connection string. if the database changes its dynamically changed
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Staffs employee = new Staffs()
                    {
                        ID = Convert.ToInt32(reader["staff_id"]),
                        Firstname = reader["first_name"].ToString(),
                        Lastname = reader["last_name"].ToString(),
                        Email = reader["email"].ToString(),
                        Phone =  Convert.ToInt32(reader["phone"]),
                        Store_ID = Convert.ToInt32(reader["store_id"]),
                        Manager_ID = Convert.ToInt32(reader["manager_id"])
                    };
                    employees.Add(employee);
                }
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc);
            }
            finally
            {
                closeConnection();
            }
            return employees; //remember to return the list
        }

        public bool updateEmployee(Staffs someEmp)
        {
            bool status = false;
            try
            {
                openConnection();
                String cmd = "update BikeStores set Name = '" + someEmp.Firstname + "', Website = '" + someEmp.Lastname + "' where id = " + someEmp.ID + ";";
                SqlCommand command = new SqlCommand(cmd, currConnection);
                command.ExecuteNonQuery();
                closeConnection();
                status = true;

            }
            catch (Exception e)
            {
                status = false;
            }
            finally
            {
                closeConnection();
            }
            return status;
        }


    }
}