using BSLCanteenAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BSLCanteenAPI.DAL
{
    public class DALReport
    {
        SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["BSL"].ConnectionString);

        #region Start Fn_Get_Coupon_Status_Report 21-Oct-2026  by Ankit

        public List<clsOrderStatusResp> Fn_Get_Coupon_Status_Report(clsOrderStatusReq objReq)
        {
            var objResp = new List<clsOrderStatusResp>();
            var obj = new clsOrderStatusResp();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Get_Coupon_Status_Report");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }


                SqlCommand cmd = new SqlCommand("USP_Get_Coupon_Status_Report", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                cmd.Parameters.AddWithValue("@OrderStatus", objReq.OrderStatus);
                cmd.Parameters.AddWithValue("@CoupIssueStartDate", objReq.CoupIssueStartDate);
                cmd.Parameters.AddWithValue("@CoupIssueEndDate", objReq.CoupIssueEndDate);
                cmd.Parameters.AddWithValue("@OrderStartDate", objReq.OrderStartDate);
                cmd.Parameters.AddWithValue("@OrderEndDate", objReq.@OrderEndDate);
                cmd.Parameters.AddWithValue("@PageNumber", objReq.PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", objReq.PageSize);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsOrderStatusResp();
                        obj.CouponId = Convert.ToInt64(ds.Tables[0].Rows[i]["CouponId"]);
                        obj.ItemId = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemId"]);
                        obj.ItemName = Convert.ToString(ds.Tables[0].Rows[i]["ItemName"]);
                        obj.Price = Convert.ToDecimal(ds.Tables[0].Rows[i]["Price"]);
                        obj.CoupIssueDate = Convert.ToString(ds.Tables[0].Rows[i]["CoupIssueDate"]);
                        obj.OrderTakenDate = Convert.ToString(ds.Tables[0].Rows[i]["OrderTakenDate"]);
                        obj.OrderStatus = Convert.ToString(ds.Tables[0].Rows[i]["OrderStatus"]);
                        
                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmpId"]);
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        obj.EmpLocation = Convert.ToString(ds.Tables[0].Rows[i]["EmpLocation"]);
                        obj.EmpDepartment = Convert.ToString(ds.Tables[0].Rows[i]["EmpDepartment"]);
                        obj.CreatedBy = Convert.ToInt32(ds.Tables[0].Rows[i]["CreatedBy"]);
                        obj.CreatedOn = Convert.ToString(ds.Tables[0].Rows[i]["CreatedOn"]);

                        string ModifiedBy = Convert.ToString(ds.Tables[0].Rows[i]["ModifiedBy"]);
                        if (ModifiedBy != "")
                        {
                            obj.ModifiedBy = Convert.ToInt32(ds.Tables[0].Rows[i]["ModifiedBy"]);
                        }
                        obj.ModifiedOn = Convert.ToString(ds.Tables[0].Rows[i]["ModifiedOn"]);

                        
                        obj.CouponType = Convert.ToString(ds.Tables[0].Rows[i]["CouponType"]);
                        obj.EmpStatus = Convert.ToBoolean(ds.Tables[0].Rows[i]["EmpStatus"]);
                        obj.TotalRows = Convert.ToInt64(ds.Tables[0].Rows[i]["TotalRows"]);

                        obj.vErrorMsg = "Success";
                        obj.vErrorCode = 200;
                        objResp.Add(obj);
                        i++;
                    }
                }
                else
                {
                    obj.vErrorMsg = "No Record Found.";
                    obj.vErrorCode = 400;
                    objResp.Add(obj);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_Coupon_Status_Report", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_Coupon_Status_Report");
            return objResp;
        }

        #endregion End Fn_Get_Coupon_Status_Report 21-Oct-2026  by Ankit
    }
}