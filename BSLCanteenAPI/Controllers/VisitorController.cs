using BSLCanteenAPI.DAL;
using BSLCanteenAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace BSLCanteenAPI.Controllers
{
    public class VisitorController : ApiController
    {
        // GET: Visitor

        DALVisitor _DALVisitor = new DALVisitor();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Visitor/Fn_Visitor_CouponGeneration")]
        public clsCouponOrder Fn_Visitor_CouponGeneration(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            objResp = _DALVisitor.Fn_Visitor_CouponGeneration(objReq);
            return objResp;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Visitor/Fn_Get_Visitor_Coupon")]
        public List<clsVisitorCoupon> Fn_Get_Visitor_Coupon(clsCouponReport objReq)
        {
            var objResp = new List<clsVisitorCoupon>();
            objResp = _DALVisitor.Fn_Get_Visitor_Coupon(objReq);
            return objResp;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Visitor/Fn_Update_Visitor_Coupon_PrintDate")]
        public clsCouponOrder Fn_Update_Visitor_Coupon_PrintDate(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            objResp = _DALVisitor.Fn_Update_Visitor_Coupon_PrintDate(objReq);
            return objResp;
        }
    }
}