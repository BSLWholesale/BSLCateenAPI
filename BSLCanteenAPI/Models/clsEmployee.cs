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
        public string CategoryIcon { get; set; }
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
        public string CouponType { get; set; }
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
        public int ItemId { get; set; }
        public string ItemCategory { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
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
        public string CouponType { get; set; }
        public string Category { get; set; }
        public string CategoryIcon { get; set; }
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
        public string EmpDepartment { get; set; }
        public string EmpLocation { get; set; }
        public decimal Price { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
        public string OrderByReport { get; set; }
        public int ItemId { get; set; }
        public bool EmpStatus { get; set; }
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
        public string EmpLocation { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class clsMonthlyReportResp
    {
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpDepartment { get; set; }
        public string CouponType { get; set; }
        public string CategoryIcon { get; set; }
        public string ItemCategory { get; set; }
        public string OrderTakenDate { get; set; }
        public int TotalCoupons { get; set; }
        public decimal TotalPrice { get; set; }
        public Int64 TotalRows { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }

    public class clsCountMenuItem
    {
        public int CountItem { get; set; }
        public int CanteenId { get; set; }
        public string EmpLocation { get; set; }
        public string CategoryIcon { get; set; }
        public string ItemCategory { get; set; }
        public string OrderTakenDate { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }


    public class clsCategoryReport
    {
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public string CurrentDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public Int32 EmpId { get; set; }
        public string Noofworkers { get; set; }
        public string GeneratedCoupons { get; set; }
        public string ScannedCoupons { get; set; }
        public string GeneratedCouponTea { get; set; }
        public string QRScannedTea { get; set; }
        public string GeneratedCouponBreakfast { get; set; }
        public string QRScannedBreakfast { get; set; }
        public string GeneratedCouponThali { get; set; }
        public string QRScannedThali { get; set; }
        public string GeneratedCouponMiniThali { get; set; }
        public string QRScannedMiniThali { get; set; }
        public string ItemCategory { get; set; }
        public string ReportType { get; set; }
        public string EmpLocation { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsShiftReport
    {
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public string ShiftName { get; set; }
        public string CurrentDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public Int32 EmpId { get; set; }
        public string Noofworkers { get; set; }
        public string GeneratedCouponTea { get; set; }
        public string QRScannedTea { get; set; }
        public string GeneratedCouponBreakfast { get; set; }
        public string QRScannedBreakfast { get; set; }
        public string GeneratedCouponThali { get; set; }
        public string QRScannedThali { get; set; }
        public string GeneratedCouponMiniThali { get; set; }
        public string QRScannedMiniThali { get; set; }
        public string ItemCatgeory { get; set; }
        public string ReportType { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsDailyMonthlyAllEmpDetailReq
    {
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int64 CouponId { get; set; }
        public string ItemCatgeory { get; set; }
        public string OrderTakenDate { get; set; }
        public string CurrentDate { get; set; }
        public string PeriodType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Int64 TotalRows { get; set; }
        public string EmpDepartment { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }

    public class clsDailyMonthlyAllEmpDetailResp
    {
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int64 CouponId { get; set; }
        public string ItemCatgeory { get; set; }
        public string OrderTakenDate { get; set; }
        public string CurrentDate { get; set; }
        public string PeriodType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Int64 TotalRows { get; set; }
        public string EmpDepartment { get; set; }
        public string ItemName { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsDailyMonthlyAllEmpSummaryReq
    {
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int64 CouponId { get; set; }
        public string ItemCategory { get; set; }
        public string OrderTakenDate { get; set; }
        public string CurrentDate { get; set; }
        public string PeriodType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Int64 TotalRows { get; set; }
        public string EmpDepartment { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsDailyMonthlyAllEmpSummaryResp
    {
        public Int32 EmpId { get; set; }
        public string EmpName { get; set; }
        public int CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Int64 CouponId { get; set; }
        public string ItemCategory { get; set; }
        public string OrderTakenDate { get; set; }
        public string CurrentDate { get; set; }
        public string PeriodType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Int64 TotalRows { get; set; }
        public string TotalCoupon { get; set; }
        public decimal Amt { get; set; }
        public string EmpDepartment { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }

    public class clsAddMenu
    {
        public int ItemId { get; set; }
        public string Category { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public bool MActive { get; set; }
        public string CategoryIcon { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string QueryType { get; set; }
        public string vErrorMsg { get; set; }
        public int vErrorCode { get; set; }
    }


    public class clsItemWiseReport
    {
        public string CurrentDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public Int32 EmpId { get; set; }
        public string BFLWorker { get; set; }
        public string BSLWorker { get; set; }
        public string BTMWorker { get; set; }
        public string BJFWorker { get; set; }
        public string FoodTruckWorker { get; set; }
        public string Mill7Worker { get; set; }
        public string TPPWorker { get; set; }
        public string Weaving4Worker { get; set; }
        public string Worsted1Worker { get; set; }
        public string ItemName { get; set; }
        public string TotalCount { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsCouponTypeReport
    {
        public string CurrentDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CouponType { get; set; }
        public Int32 EmpId { get; set; }
        public string BFLWorker { get; set; }
        public string BSLWorker { get; set; }
        public string BTMWorker { get; set; }
        public string BJFWorker { get; set; }
        public string FoodTruckWorker { get; set; }
        public string Mill7Worker { get; set; }
        public string TPPWorker { get; set; }
        public string Weaving4Worker { get; set; }
        public string Worsted1Worker { get; set; }
        public string TotalCount { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsUPIDataUpload
    {
        public string TransactionDate { get; set; }
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public Int32 Qty { get; set;}
        public string LocationID { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }

    public class clsUPIDataReport
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TransactionDate { get; set; }
        public string TrasactionID { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public Int32 Qty { get; set; }
        public string LocationID { get; set; }
        public Int32 TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<string, decimal> DateWiseQty { get; set; }
        public string BFL { get; set; }
        public string BSL { get; set; }
        public string BTM { get; set; }
        public string CKDININGBANSWARA { get; set; }
        public string TPPDyeHouse { get; set; }
        public string W04 { get; set; }
        public string WEAVING7 { get; set; }
        public string WorstedVan { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }



    public class clsUPIMonthlyReportRequest
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }


    public class clsUPIMonthlyReportResponse
    {
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public Int32 Qty { get; set; }
        public string LocationID { get; set; }
        public Int32 TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<string, decimal> DateWiseQty { get; set; }
        public int vErrorCode { get; set; }
        public string vErrorMsg { get; set; }
    }


}