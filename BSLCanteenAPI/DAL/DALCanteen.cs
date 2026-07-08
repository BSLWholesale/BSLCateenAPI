using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using BSLCanteenAPI.DAL;
using BSLCanteenAPI.Models;

namespace BSLCanteenAPI.DAL
{
    public class DALCanteen
    {

        SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["BSL"].ConnectionString);

        Int64 CouponID;
        public clsCouponOrder Fn_Emp_CouponGeneration(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            try
            {
                if (objReq.EmpId == null || objReq.EmpId == 0)
                {
                    objResp.vErrorMsg = "Please Select the Valid Employee ID";
                }
                else if (objReq.CanteenId == null || objReq.CanteenId == 0)
                {
                    objResp.vErrorMsg = "Please Select the Canteen ID";
                }
                else if (objReq.Items == null || objReq.Items.Count == 0)
                {
                    objResp.vErrorMsg = "Please Select At Least One Item.";
                }
                else
                {
                    if (Con.State == ConnectionState.Broken)
                    { Con.Close(); }
                    if (Con.State == ConnectionState.Closed)
                    { Con.Open(); }

                    foreach (var item in objReq.Items)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {
                            DALCanteen _DALCanteen = new DALCanteen();
                            CouponID = _DALCanteen.Fn_Get_Max_CouponId(objReq);

                            SqlCommand cmd = new SqlCommand("USP_Canteen", Con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CouponId", CouponID);
                            cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                            cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                            cmd.Parameters.AddWithValue("@ItemCategory", item.ItemCategory);
                            cmd.Parameters.AddWithValue("@CreatedBy", objReq.CreatedBy);
                            cmd.Parameters.AddWithValue("@OrderStatus", "Generated");
                            cmd.Parameters.AddWithValue("@QueryType", "InsertCouponId");

                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                objResp.vErrorMsg = "Coupons Generated Successfully";
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
                Logger.WriteLog("Function Name : Fn_Emp_CouponGeneration", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            return objResp;
        }


        public Int64 Fn_Get_Max_CouponId(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT CONCAT(FORMAT(GETDATE(),'ddMMyyyy'),SUBSTRING(FORMAT(ISNULL(MAX(CouponId)+1,1),'00000000000000'),9,6)) FROM CouponOrder2026 WHERE CONVERT(DATE, CreatedOn)= CONVERT(DATE, GETDATE())";

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
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_Max_CouponId", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
            }
            return CouponID;
        }


        public List<clsCouponOrder> Fn_Fetch_EmpCouponId(clsCouponOrder objReq)
        {
            var objResp = new List<clsCouponOrder>();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Canteen", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@QueryType", "FetchCouponID");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var objItem = new clsCouponOrder();
                        objItem.CouponId = Convert.ToInt64(ds.Tables[0].Rows[i]["CouponId"]);
                        objItem.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i]["ItemCategory"]);
                        objItem.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmpId"]);
                        objItem.CouponIssueDate = Convert.ToString(ds.Tables[0].Rows[i]["CoupIssueDate"]);
                        objItem.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CantId"]);
                        objItem.vErrorMsg = "Success";
                        objItem.vErrorCode = 200;
                        objResp.Add(objItem);
                        i++;
                    }
                }
                else
                {
                    var objItem = new clsCouponOrder();
                    objItem.vErrorMsg = "Coupon ID records are not Found.";
                    objItem.vErrorCode = 400;
                    objResp.Add(objItem);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Fetch_EmpCouponId", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                var objItem = new clsCouponOrder();
                objItem.vErrorMsg = exp.Message.ToString();
                objItem.vErrorCode = 500;
                objResp.Add(objItem);
            }
            finally
            {
                Con.Close();
            }
            return objResp;
        }


        public List<clsCouponReport> Fn_Get_Coupon_Order(clsCouponReport objReq)
        {
            var objResp = new List<clsCouponReport>();
            var obj = new clsCouponReport();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT CouponId, ItemCategory, CoupIssueDate, CoupIssueTime, OrdTakenDate, OrdTakenTime, OrdStatus, CanteenId, CanteenName, ";
                strSql = strSql + " EmployeeId, EmpName, CreatedBy  FROM  vCouponOrder WHERE 1=1 ";
                if (objReq.EmpId != 0 && objReq.EmpId != null)
                {
                    strSql = strSql + " AND EmployeeId = @EmpId ";
                }
                if (objReq.CouponId != 0 && objReq.CouponId != null)
                {
                    strSql = strSql + " AND CouponId = @CouponId ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    strSql = strSql + " AND ItemCategory = @ItemCategory ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND OrdTakenDate = @OrderTakenDate ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderStatus))
                {
                    strSql = strSql + " AND OrdStatus = @OrderStatus ";
                }
                strSql = strSql + " ORDER BY OrdTakenDate, CoupIssueDate DESC ";
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;
                if (objReq.EmpId != 0 && objReq.EmpId != null)
                {
                    cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                }
                if (objReq.CouponId != 0 && objReq.CouponId != null)
                {
                    cmd.Parameters.AddWithValue("@CouponId", objReq.CouponId);
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    cmd.Parameters.AddWithValue("@ItemCategory", objReq.ItemCategory);
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    cmd.Parameters.AddWithValue("@OrderTakenDate", objReq.OrderTakenDate);
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderStatus))
                {
                    cmd.Parameters.AddWithValue("@OrderStatus", objReq.OrderStatus);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsCouponReport();
                        obj.CouponId = Convert.ToInt64(ds.Tables[0].Rows[i]["CouponId"]);
                        obj.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i]["ItemCategory"]);
                        obj.CouponIssueDate = Convert.ToString(ds.Tables[0].Rows[i]["CoupIssueDate"]);
                        obj.CouponIssueTime = Convert.ToString(ds.Tables[0].Rows[i]["CoupIssueTime"]);
                        obj.OrderTakenDate = Convert.ToString(ds.Tables[0].Rows[i]["OrdTakenDate"]);
                        obj.OrderTakenTime = Convert.ToString(ds.Tables[0].Rows[i]["OrdTakenTime"]);
                        obj.OrderStatus = Convert.ToString(ds.Tables[0].Rows[i]["OrdStatus"]);
                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeId"]);
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        obj.CreatedBy = Convert.ToInt32(ds.Tables[0].Rows[i]["CreatedBy"]);
                        obj.vErrorMsg = "Success";
                        obj.vErrorCode = 200;
                        objResp.Add(obj);
                        i++;
                    }
                }
                else
                {
                    obj = new clsCouponReport();
                    obj.vErrorMsg = "No Record Found.";
                    obj.vErrorCode = 400;
                    objResp.Add(obj);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_Coupon_Order", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                obj = new clsCouponReport();
                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            return objResp;
        }

        public clsCouponReport Fn_ProcessCouponTransaction(clsCouponReport objReq)
        {
            var objResp = new clsCouponReport();
            var objCheck = new List<clsCouponReport>();
            var obj = new clsCouponReport();
            obj.CouponId = objReq.CouponId;
            objCheck = Fn_Get_Coupon_Order(obj);
            try
            {
                if (objReq.CouponId == null || objReq.CouponId == 0)
                {
                    objResp.vErrorMsg = "Please Send CouponId";
                    objResp.vErrorCode = 400;
                }
               else if (objReq.CouponId != objCheck[0].CouponId)
                {
                    objResp.vErrorMsg = "Invalid CouponId";
                    objResp.vErrorCode = 400;
                }
                else if (objReq.CanteenId != objCheck[0].CanteenId)
                {
                    objResp.vErrorMsg = "Wrong canteen";
                    objResp.vErrorCode = 400;
                }
                else if (objReq.ItemCategory != objCheck[0].ItemCategory)
                {
                    objResp.vErrorMsg = "Please Select Correct ItemCategory";
                    objResp.vErrorCode = 400;
                }
                else if (objCheck[0].OrderStatus == "Scanned")
                {
                    objResp.vErrorMsg = "Coupon already scanned";
                    objResp.vErrorCode = 400;
                }
               else if (objCheck[0].OrderStatus == "Cancel")
                {
                    objResp.vErrorMsg = "Coupon cancelled";
                    objResp.vErrorCode = 400;
                }
                else
                {
                    if (Con.State == ConnectionState.Broken)
                    { Con.Close(); }
                    if (Con.State == ConnectionState.Closed)
                    { Con.Open(); }

                    SqlCommand cmd = new SqlCommand("USP_Canteen", Con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CouponId", objReq.CouponId);
                    cmd.Parameters.AddWithValue("@ItemCategory", objReq.ItemCategory);
                    cmd.Parameters.AddWithValue("@CreatedBy", objReq.CreatedBy);
                    cmd.Parameters.AddWithValue("@OrderStatus", "Scanned");
                    cmd.Parameters.AddWithValue("@QueryType", "TransactionCouponId");
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        objResp.vErrorMsg = "Success";
                        objResp.vErrorCode = 200;
                    }
                    else
                    {
                        objResp.vErrorMsg = "Erorr in  coupon scanning";
                        objResp.vErrorCode = 400;
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_ProcessCouponTransaction", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            return objResp;
        }


        public List<clsCouponReport> Fn_Fetch_EmpReportSummary(clsCouponReport objReq)
        {
            var objResp = new List<clsCouponReport>();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("", objReq.EmpId);
                cmd.Parameters.AddWithValue("", "");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var objItem = new clsCouponReport();
                        objItem.CouponId = Convert.ToInt64(ds.Tables[0].Rows[i][""]);
                        objItem.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i][""]);
                        objItem.CanteenName = Convert.ToString(ds.Tables[0].Rows[i][""]);
                        objItem.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i][""]);
                        objItem.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i][""]);
                        objItem.vErrorMsg = "Success";
                        objItem.vErrorCode = 200;
                        objResp.Add(objItem);
                        i++;
                    }
                }
                else
                {
                    var objItem = new clsCouponReport();
                    objItem.vErrorMsg = "Employee Summary is not found.";
                    objItem.vErrorCode = 400;
                    objResp.Add(objItem);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Fetch_EmpReportSummary", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                var objItem = new clsCouponReport();
                objItem.vErrorMsg = exp.Message.ToString();
                objItem.vErrorCode = 500;
                objResp.Add(objItem);
            }
            finally
            {
                Con.Close();
            }
            return objResp;
        }


    }
}