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
        public string Location { get; set; }
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
        public string ItemName { get; set; }
        public string Price { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string vErrorMsg { get; set; }
    }


    public class clsCouponOrder
    {
        public Int64 CouponId { get; set; }
        public Int32 EmpId { get; set; }
        public string CouponIssueDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderTakenDate { get; set; }
        public string CanteenId { get; set; }
        public string ItemCategory { get; set; }
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


}