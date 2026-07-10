using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using BSLCanteenAPI.DAL;
using BSLCanteenAPI.Models;

namespace BSLCanteenAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        // GET: Employee

        DALEmployee _DALEmployee = new DALEmployee();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Login_Employee")]
        public clsEmployee Fn_Login_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            objResp = _DALEmployee.Fn_Login_Employee(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Insert_Employee")]
        public clsEmployee Fn_Insert_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            objResp = _DALEmployee.Fn_Insert_Employee(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Fetch_EmployeeDetails")]
        public List<clsEmployee> Fn_Fetch_EmployeeDetails(clsEmployee objReq)
        {
            var objResp = new List<clsEmployee>();
            objResp = _DALEmployee.Fn_Fetch_EmployeeDetails(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Update_EmployeeDetails")]
        public clsEmployee Fn_Update_EmployeeDetails(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            objResp = _DALEmployee.Fn_Update_EmployeeDetails(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Delete_Employee")]
        public clsEmployee Fn_Delete_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            objResp = _DALEmployee.Fn_Delete_Employee(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Fetch_All_Employee")]
        public List<clsEmployee> Fn_Fetch_All_Employee(clsEmployee objReq)
        {
            var objResp = new List<clsEmployee>();
            objResp = _DALEmployee.Fn_Fetch_All_Employee(objReq);
            return objResp;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Fill_DropdownList")]
        public List<clsResponseDropdown> Fn_Fill_DropdownList(clsRequestDropdown objReq)
        {
            var objResp = new List<clsResponseDropdown>();
            objResp = _DALEmployee.Fn_Fill_DropdownList(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Upload_BulkEmployeeData")]
        public List<clsEmployee> Fn_Upload_BulkEmployeeData(List<clsEmployee> emplist)
        {
            var objResp = new List<clsEmployee>();
            int success = 0;
            int failed = 0;

            foreach (var emp in emplist)
            {
                var result = Fn_Upload_BulkEmployeeData_List(emp);
                if (result.vErrorMsg == "Sucess")
                {
                    success++;
                }
                else
                {
                    failed++;
                }
            }
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Employee/Fn_Upload_BulkEmployeeData_List")]
        public clsEmployee Fn_Upload_BulkEmployeeData_List(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            objResp = _DALEmployee.Fn_Upload_BulkEmployeeData_List(objReq);
            return objResp;
        }


    }
}