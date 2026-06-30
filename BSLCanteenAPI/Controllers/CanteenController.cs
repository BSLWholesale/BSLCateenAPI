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
    public class CanteenController : ApiController
    {
        // GET: Canteen

        DALCanteen _DALCanteen = new DALCanteen();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Canteen/Fn_Emp_CouponGeneration")]
        public clsCouponOrder Fn_Emp_CouponGeneration(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            objResp = _DALCanteen.Fn_Emp_CouponGeneration(objReq);
            return objResp;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Canteen/Fn_Fetch_EmpCouponId")]
        public List<clsCouponOrder> Fn_Fetch_EmpCouponId(clsCouponOrder objReq)
        {
            var objResp = new List<clsCouponOrder>();
            objResp = _DALCanteen.Fn_Fetch_EmpCouponId(objReq);
            return objResp;
        }


    }
}