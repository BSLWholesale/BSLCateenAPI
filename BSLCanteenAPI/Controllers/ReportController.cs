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
    public class ReportController : ApiController
    {
        // GET: Report

        DALReport _DALReport = new DALReport();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Report/Fn_Get_Coupon_Status_Report")]
        public List<clsOrderStatusResp> Fn_Get_Coupon_Status_Report(clsOrderStatusReq objReq)
        {
            var objResp = new List<clsOrderStatusResp>();
            objResp = _DALReport.Fn_Get_Coupon_Status_Report(objReq);
            return objResp;
        }
    }
}