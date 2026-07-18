using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSLCanteenAPI.Models
{
    public class clsEmployee
    {
        public Int64 EmpId { get; set; }
        public string EmpName { get; set; }
        public string Department { get; set; }
        public string Salary { get; set; }
        public string EmpPassword { get; set; }
        public string EmpMobile { get; set; }
        public string EmpLocation { get; set; }
        public string EmpRole { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public string LoginID { get; set; }
        public bool EmpStatus { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }


    public class clMenuItems
    {
        public Int32 ItemId { get; set; }
        public string Category { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }


    public class clsCouponOrder
    {
        public Int64 CouponId { get; set; }
        public Int32 EmpId { get; set; }
        public Int32 RowIndex { get; set; }
        public string CouponIssueDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderTakenDate { get; set; }
        public int CanteenId { get; set; }
        public string ItemCategory { get; set; }
        public string EmpLocation { get; set; }
        public List<clsCouponItem> Items { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }

    }
   

    public class clsCanteen
    {
        public string CanteenId { get; set; }
        public string CanteenName { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsCouponItem
    {
        public string ItemCategory { get; set; }
        public int Quantity { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }

    public class clsRequestDropdown
    {
        public string vFieldName { get; set; }
        public string vValueField { get; set; }
        public string vTBLName { get; set; }
        public string vCriteria { get; set; }
        public string vErrorMsg { get; set; }
    }
    public class clsResponseDropdown
    {
        public string vFieldName { get; set; }
        public string vValueField { get; set; }
        public string vErrorMsg { get; set; }
    }

    public class clsCouponReport
    {
        public Int64 CouponId { get; set; }
        public Int32 RowIndex { get; set; }
        public string ItemCategory { get; set; }
        public string CouponIssueDate { get; set; }
        public string CouponIssueTime { get; set; }
        public string OrderTakenDate { get; set; }
        public string OrderTakenTime { get; set; }
        public string OrderStatus { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpLocation { get; set; }
        public decimal Price { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
        public string OrderByReport { get; set; }
    }

    public class clsMonthlyReportReq
    {
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int32 EmpId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string OrderTakenDate { get; set; }
        public string OrderStatus { get; set; }
        public string ItemCategory { get; set; }
        public string ReportType { get; set; }
    }
    public class clsMonthlyReportResp
    {
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public string ItemCategory { get; set; }
        public string OrderTakenDate { get; set; }
        public int TotalCoupons { get; set; }
        public decimal TotalPrice { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }

    public class clsCountMenuItem
    {
        public int CountItem { get; set; }
        public int CanteenId { get; set; }
        public string EmpLocation { get; set; }
        public string ItemCategory { get; set; }
        public string OrderTakenDate { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }
}