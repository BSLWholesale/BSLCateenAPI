using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSLCanteenAPI.Models
{
    public class clsReport
    {
    }

    public class clsOrderStatusReq
    {
        public int CanteenId { get; set; }
        public string OrderStatus { get; set; }
        public string CoupIssueStartDate { get; set; }
        public string CoupIssueEndDate { get; set; }
        public string OrderStartDate { get; set; }
        public string @OrderEndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }

    public class clsOrderStatusResp
    {
        public Int64 CouponId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string CoupIssueDate { get; set; }
        public string OrderTakenDate { get; set; }
        public string OrderStatus { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpLocation { get; set; }
        public string EmpDepartment { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string CouponType { get; set; }
        public bool EmpStatus { get; set; }
        public Int64 TotalRows { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }

    }
}