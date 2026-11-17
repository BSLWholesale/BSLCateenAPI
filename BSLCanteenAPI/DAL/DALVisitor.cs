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
    public class DALVisitor
    {

        SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["BSL"].ConnectionString);

        int currentYear = DateTime.Now.Year;
        Int64 CouponID; int mxID;       


        public Int64 Fn_Get_Max_CouponId(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            try
            {
                ////if (Con.State == ConnectionState.Broken)
                ////{ Con.Close(); }
                //if (Con.State == ConnectionState.Closed)
                //{ Con.Open(); }

                string strSql = "SELECT CONCAT(FORMAT(GETDATE(),'ddMMyyyy'),SUBSTRING(FORMAT(ISNULL(MAX(CouponId)+1,1),'00000000000000'),9,6)) FROM CouponOrder" + currentYear + "  WHERE CONVERT(DATE, CreatedOn)= CONVERT(DATE, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(strSql, Con))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        CouponID = Convert.ToInt64(result);
                    }
                    else
                    {
                        CouponID = Convert.ToInt64(DateTime.Now.ToString("ddMMyyyy") + "000001");
                    }
                    //Con.Close();
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_Max_CouponId", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
            }
            return CouponID;
        }

        public Int32 Fn_Get_MXID(string strTBLName, string strFieldName, string strCriteria)
        {
            try
            {
                //if (Con.State == ConnectionState.Broken)
                //{ Con.Close(); }
                //if (Con.State == ConnectionState.Closed)
                //{ Con.Open(); }

                string strSql = "SELECT MAX(" + strFieldName + ") AS ID FROM " + strTBLName + " WHERE 1=1";
                if (!String.IsNullOrWhiteSpace(strCriteria))
                {
                    strSql = strSql + strCriteria;
                }
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        string strMXID = Convert.ToString(ds.Tables[0].Rows[i]["ID"]);
                        if (strMXID == "")
                        {
                            mxID = 1;
                        }
                        else
                        {
                            mxID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]) + 1;
                        }
                        i++;
                    }
                }
                else
                {
                    mxID = 1;
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_MXID", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                exp.Message.ToString();
            }
            return mxID;
        }

        public clsCouponOrder Fn_Visitor_CouponGeneration(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Visitor_CouponGeneration");
            try
            {
                if (objReq.EmpId == null || objReq.EmpId == 0)
                {
                    objResp.vErrorMsg = "Please Select the Valid Employee ID";
                    objResp.vErrorCode = 400;
                }
                else if (objReq.CanteenId == null || objReq.CanteenId == 0)
                {
                    objResp.vErrorMsg = "Please Select the Canteen ID";
                    objResp.vErrorCode = 400;
                }
                else if (String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    objResp.vErrorMsg = "Please Select EmpLocation";
                    objResp.vErrorCode = 400;
                }
                else if (objReq.Items == null || objReq.Items.Count == 0)
                {
                    objResp.vErrorMsg = "Please Select At Least One Item";
                    objResp.vErrorCode = 400;
                }
                else
                {
                    if (Con.State == ConnectionState.Broken)
                    { Con.Close(); }
                    if (Con.State == ConnectionState.Closed)
                    { Con.Open(); }

                    string strCriteria = " AND EmpId=" + objReq.EmpId + " AND CAST(CreatedOn AS DATE) ='" + objReq.CreatedOn + "'";
                    objReq.RowIndex = Fn_Get_MXID("VisitorCoupon" + currentYear, "RowIndex", strCriteria);

                    foreach (var item in objReq.Items)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {
                            DALCanteen _DALCanteen = new DALCanteen();
                            CouponID = Fn_Get_Max_CouponId(objReq);

                            SqlCommand cmd = new SqlCommand("USP_VisitorCoupon", Con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CouponId", CouponID);
                            cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                            cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                            cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                            cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                            cmd.Parameters.AddWithValue("@Price", item.Price);
                            cmd.Parameters.AddWithValue("@CreatedBy", objReq.CreatedBy);
                            cmd.Parameters.AddWithValue("@RowIndex", objReq.RowIndex);
                            cmd.Parameters.AddWithValue("@VisitorType", objReq.CouponType);
                            cmd.Parameters.AddWithValue("@CouponIssueDate", objReq.CouponIssueDate);
                            cmd.Parameters.AddWithValue("@QueryType", "InsertVisitorCoupon");

                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                objResp.vErrorMsg = "Success";
                                objResp.RowIndex = objReq.RowIndex;
                                objResp.vErrorCode = 200;
                            }
                            else
                            {
                                objResp.vErrorMsg = "Failed to Generate Coupon ID";
                                objResp.vErrorCode = 400;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Visitor_CouponGeneration", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Visitor_CouponGeneration");
            return objResp;
        }

        public List<clsVisitorCoupon> Fn_Get_Visitor_Coupon(clsCouponReport objReq)
        {
            var objResp = new List<clsVisitorCoupon>();
            var obj = new clsVisitorCoupon();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Get_Visitor_Coupon");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }


                SqlCommand cmd = new SqlCommand("USP_VisitorCoupon", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@RowIndex", objReq.RowIndex);
                cmd.Parameters.AddWithValue("@CouponId", objReq.CouponId);
                cmd.Parameters.AddWithValue("@CouponIssueDate", objReq.CouponIssueDate);
                cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                cmd.Parameters.AddWithValue("@VisitorType", objReq.CouponType);
                cmd.Parameters.AddWithValue("@QueryType", objReq.QueryType);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsVisitorCoupon();
                        obj.CouponId = Convert.ToInt64(ds.Tables[0].Rows[i]["CouponId"]);
                        obj.ItemId = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemId"]);
                        obj.ItemName = Convert.ToString(ds.Tables[0].Rows[i]["ItemName"]);
                        obj.CategoryIcon = Convert.ToString(ds.Tables[0].Rows[i]["CategoryIcon"]);
                        obj.Price = Convert.ToInt32(ds.Tables[0].Rows[i]["Price"]);
                        obj.CouponIssueDate = Convert.ToString(ds.Tables[0].Rows[i]["CoupIssueDate"]);
                        obj.CouponIssueTime = Convert.ToString(ds.Tables[0].Rows[i]["CoupIssueTime"]);
                        
                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeId"]);
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        obj.EmpLocation = Convert.ToString(ds.Tables[0].Rows[i]["EmpLocation"]);
                        obj.EmpDepartment = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);
                        obj.VisitorName = Convert.ToString(ds.Tables[0].Rows[i]["VisitorName"]);
                        obj.VisitorMobile = Convert.ToString(ds.Tables[0].Rows[i]["VisitorMobile"]);
                        obj.RowIndex = Convert.ToInt32(ds.Tables[0].Rows[i]["RowIndex"]);
                        obj.VisitorType = Convert.ToString(ds.Tables[0].Rows[i]["VisitorType"]);
                        obj.CreatedBy = Convert.ToInt32(ds.Tables[0].Rows[i]["CreatedBy"]);
                        obj.CreatedOn = Convert.ToString(ds.Tables[0].Rows[i]["CreatedOn"]);
                        obj.PrintDate = Convert.ToString(ds.Tables[0].Rows[i]["PrintDate"]);


                        obj.vErrorMsg = "Success";
                        obj.vErrorCode = 200;
                        objResp.Add(obj);
                        i++;
                    }
                }
                else
                {
                    obj = new clsVisitorCoupon();
                    obj.vErrorMsg = "No Record Found.";
                    obj.vErrorCode = 400;
                    objResp.Add(obj);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_Visitor_Coupon", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                obj = new clsVisitorCoupon();
                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            // Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_Coupon_Order");
            return objResp;
        }

        public clsCouponOrder Fn_Update_Visitor_Coupon_PrintDate(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Update_Visitor_Coupon_PrintDate");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }


                SqlCommand cmd = new SqlCommand("USP_VisitorCoupon", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                cmd.Parameters.AddWithValue("@VisitorType", objReq.CouponType);
                cmd.Parameters.AddWithValue("@CouponIssueDate", objReq.CouponIssueDate);
                cmd.Parameters.AddWithValue("@RowIndex", objReq.RowIndex);
                cmd.Parameters.AddWithValue("@CreatedBy", objReq.CreatedBy);
                cmd.Parameters.AddWithValue("@QueryType", "UpdatePrintDate");
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    objResp.vErrorMsg = "Success";
                    objResp.RowIndex = objReq.RowIndex;
                    objResp.vErrorCode = 200;
                }
                else
                {
                    objResp.vErrorMsg = "Print date updating failed";
                    objResp.vErrorCode = 400;
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Update_Visitor_Coupon_PrintDate", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            // Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_Coupon_Order");
            return objResp;
        }
    }
}