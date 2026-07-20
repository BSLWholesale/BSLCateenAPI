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
using Newtonsoft.Json;

namespace BSLCanteenAPI.DAL
{
    public class DALCanteen
    {

        SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["BSL"].ConnectionString);

        Int64 CouponID; int mxID;
        public clsCouponOrder Fn_Emp_CouponGeneration(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Emp_CouponGeneration");
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

                    string strCriteria = " AND EmpId=" + objReq.EmpId + " AND FORMAT(CreatedOn, 'dd-MMM-yyyy')='" + objReq.CreatedOn + "'";
                   objReq.RowIndex = Fn_Get_MXID("CouponOrder2026", "RowIndex", strCriteria);

                    foreach (var item in objReq.Items)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {
                            DALCanteen _DALCanteen = new DALCanteen();
                            CouponID = Fn_Get_Max_CouponId(objReq);

                            SqlCommand cmd = new SqlCommand("USP_Canteen", Con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CouponId", CouponID);
                            cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                            cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                            cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                            cmd.Parameters.AddWithValue("@ItemCategory", item.ItemCategory);
                            cmd.Parameters.AddWithValue("@CreatedBy", objReq.CreatedBy);
                            cmd.Parameters.AddWithValue("@RowIndex", objReq.RowIndex);
                            cmd.Parameters.AddWithValue("@OrderStatus", "Generated");
                            cmd.Parameters.AddWithValue("@QueryType", "InsertCouponId");

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
                Logger.WriteLog("Function Name : Fn_Emp_CouponGeneration", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Emp_CouponGeneration");
            return objResp;
        }


        public Int64 Fn_Get_Max_CouponId(clsCouponOrder objReq)
        {
            var objResp = new clsCouponOrder();
            try
            {
                ////if (Con.State == ConnectionState.Broken)
                ////{ Con.Close(); }
                //if (Con.State == ConnectionState.Closed)
                //{ Con.Open(); }

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


        public List<clsCouponOrder> Fn_Fetch_EmpCouponId(clsCouponOrder objReq)
        {
            var objResp = new List<clsCouponOrder>();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Fetch_EmpCouponId");
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
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Emp_CouponGeneration");
            return objResp;
        }


        public List<clsCouponReport> Fn_Get_Coupon_Order(clsCouponReport objReq)
        {
            var objResp = new List<clsCouponReport>();
            var obj = new clsCouponReport();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Get_Coupon_Order");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT CouponId, ItemCategory, Price, CoupIssueDate, CoupIssueTime, OrdTakenDate, OrdTakenTime, OrdStatus, CanteenId, CanteenName, ";
                strSql = strSql + " EmployeeId, EmpName, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, RowIndex FROM vCouponOrder WHERE 1=1 ";
                if (objReq.EmpId != 0 && objReq.EmpId != null)
                {
                    strSql = strSql + " AND EmployeeId = @EmpId ";
                }
                if (objReq.RowIndex != 0 )
                {
                    strSql = strSql + " AND RowIndex = @RowIndex ";
                }
                if (objReq.CouponId != 0 && objReq.CouponId != null)
                {
                    strSql = strSql + " AND CouponId = @CouponId ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    strSql = strSql + " AND ItemCategory = @ItemCategory ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.CouponIssueDate))
                {
                    strSql = strSql + " AND CoupIssueDate = @CouponIssueDate ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND OrdTakenDate = @OrderTakenDate ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderStatus))
                {
                    strSql = strSql + " AND OrdStatus = @OrderStatus ";
                }
                if (objReq.ModifiedBy != 0 && objReq.ModifiedBy != null)
                {
                    strSql = strSql + " AND ModifiedBy = @ModifiedBy";
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    strSql = strSql + " AND EmpLocation = @EmpLocation ";
                }
                strSql = strSql + " ORDER BY ModifiedOn DESC  ";
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;
                if (objReq.EmpId != 0 && objReq.EmpId != null)
                {
                    cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                }
                if (objReq.RowIndex != 0)
                {
                    cmd.Parameters.AddWithValue("@RowIndex", objReq.RowIndex);
                }
                if (objReq.CouponId != 0 && objReq.CouponId != null)
                {
                    cmd.Parameters.AddWithValue("@CouponId", objReq.CouponId);
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    cmd.Parameters.AddWithValue("@ItemCategory", objReq.ItemCategory);
                }
                if (!String.IsNullOrWhiteSpace(objReq.CouponIssueDate))
                {
                    cmd.Parameters.AddWithValue("@CouponIssueDate", objReq.CouponIssueDate);
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    cmd.Parameters.AddWithValue("@OrderTakenDate", objReq.OrderTakenDate);
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderStatus))
                {
                    cmd.Parameters.AddWithValue("@OrderStatus", objReq.OrderStatus);
                }
                if (objReq.ModifiedBy != 0 && objReq.ModifiedBy != null)
                {
                    cmd.Parameters.AddWithValue("@ModifiedBy", objReq.ModifiedBy);
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
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
                        obj.Price = Convert.ToInt32(ds.Tables[0].Rows[i]["Price"]);
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
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_Coupon_Order");
            return objResp;
        }

        public clsCouponReport Fn_ProcessCouponTransaction(clsCouponReport objReq)
        {
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_ProcessCouponTransaction");
            var objResp = new clsCouponReport();
            var objCheck = new List<clsCouponReport>();
            var obj = new clsCouponReport();
            obj.CouponId = objReq.CouponId;
            obj.CanteenId = objReq.CanteenId;
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
                    cmd.Parameters.AddWithValue("@ModifiedBy", objReq.ModifiedBy);
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
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_ProcessCouponTransaction");
            return objResp;
        }

        public List<clsCountMenuItem> Fn_Get_Count_ItemMenu(clsCountMenuItem objReq)
        {
            var objResp = new List<clsCountMenuItem>();
            var obj = new clsCountMenuItem();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Get_Count_ItemMenu");
            try
            {
                string strSql = "";
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                strSql = "SELECT C.ItemCategory, ISNULL(COUNT(O.CouponId), 0) AS RC ";
                strSql = strSql + " FROM ( SELECT 'Tea' AS ItemCategory UNION ALL SELECT 'Breakfast' UNION ALL SELECT 'Thali' ";
                strSql = strSql + " UNION ALL SELECT 'Mini Thali' ) C ";
                strSql = strSql + " LEFT JOIN vCouponOrder O  ON O.ItemCategory = C.ItemCategory AND O.OrdStatus = 'Scanned' ";
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND O.OrdTakenDate='" + objReq.OrderTakenDate + "'";
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    strSql = strSql + " AND O.EmpLocation='" + objReq.EmpLocation + "'";
                }
                if (objReq.CanteenId != null && objReq.CanteenId != 0)
                {
                    strSql = strSql + " AND O.CanteenId=" + objReq.CanteenId + "";
                }
                strSql = strSql + " GROUP BY  C.ItemCategory ";
                SqlDataAdapter da = new SqlDataAdapter(strSql, Con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {

                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsCountMenuItem();
                        obj.CountItem = Convert.ToInt32(ds.Tables[0].Rows[i]["RC"]);
                        obj.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i]["ItemCategory"]);
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
                Logger.WriteLog("Function Name : Fn_Get_Count_ItemMenu", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_Count_ItemMenu");
            return objResp;
        }

        public List<clsCouponReport> Fn_Fetch_EmpReportSummary(clsCouponReport objReq)
        {
            var objResp = new List<clsCouponReport>();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Fetch_EmpReportSummary");
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
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Fetch_EmpReportSummary");
            return objResp;
        }

        public List<clsCouponItem> Fn_Get_All_Category(clsCouponItem objReq)
        {
            var objResp = new List<clsCouponItem>();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Get_All_Category");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_GetMenu", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "SelectCategory");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var objItem = new clsCouponItem();
                        objItem.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i]["Category"]);
                        objItem.vErrorMsg = "Success";
                        objItem.vErrorCode = 200;
                        objResp.Add(objItem);
                        i++;
                    }
                }
                else
                {
                    var objItem = new clsCouponItem();
                    objItem.vErrorMsg = "ItemCategory not found.";
                    objItem.vErrorCode = 400;
                    objResp.Add(objItem);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_All_Category", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                var objItem = new clsCouponItem();
                objItem.vErrorMsg = exp.Message.ToString();
                objItem.vErrorCode = 500;
                objResp.Add(objItem);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_All_Category");
            return objResp;
        }

        public List<clMenuItems> Fn_Get_All_MenuItems(clMenuItems objReq)
        {
            var objResp = new List<clMenuItems>();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Get_All_MenuItems");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_GetMenu", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Category", objReq.Category);
                cmd.Parameters.AddWithValue("@QueryType", "Select_MenuItem");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var objItem = new clMenuItems();
                        objItem.ItemId = Convert.ToInt32(ds.Tables[0].Rows[i]["ItemId"]);
                        objItem.ItemName = Convert.ToString(ds.Tables[0].Rows[i]["ItemName"]);
                        objItem.Category = Convert.ToString(ds.Tables[0].Rows[i]["Category"]);
                        objItem.Price = Convert.ToDecimal(ds.Tables[0].Rows[i]["Price"]);
                        objItem.Category = Convert.ToString(ds.Tables[0].Rows[i]["Category"]);
                        objItem.vErrorMsg = "Success";
                        objItem.vErrorCode = 200;
                        objResp.Add(objItem);
                        i++;
                    }
                }
                else
                {
                    var objItem = new clMenuItems();
                    objItem.vErrorMsg = "Menu item not found.";
                    objItem.vErrorCode = 400;
                    objResp.Add(objItem);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Get_All_Category", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                var objItem = new clMenuItems();
                objItem.vErrorMsg = exp.Message.ToString();
                objItem.vErrorCode = 500;
                objResp.Add(objItem);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Get_All_MenuItems");
            return objResp;
        }

        public List<clsMonthlyReportResp> Fn_CanteenWise_Report(clsMonthlyReportReq objReq)
        {
            var objResp = new List<clsMonthlyReportResp>();
            var obj = new clsMonthlyReportResp();
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Request", "Fn_CanteenWise_Report");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT CanteenId, CanteenName, ItemCategory, OrdTakenDate, COUNT(CouponId) AS TotalCoupons, ";
                strSql = strSql + " SUM(Price) AS TotalPrice FROM vCouponOrder WHERE 1=1 AND OrdStatus = 'Scanned' ";
                
                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    strSql = strSql + " AND CanteenId = @CanteenId ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    strSql = strSql + " AND CanteenName = @CanteenName ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    strSql = strSql + " AND ItemCategory = @ItemCategory ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    strSql = strSql + " AND EmpLocation = @EmpLocation ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND OrdTakenDate='" + objReq.OrderTakenDate + "'";
                }
                if (!String.IsNullOrWhiteSpace(objReq.FromDate) && !String.IsNullOrWhiteSpace(objReq.ToDate))
                {
                    strSql = strSql + " AND OrdTakenDate BETWEEN '" + objReq.FromDate + "' AND '" + objReq.ToDate + "'";
                }                
                strSql = strSql + " GROUP BY  CanteenId, CanteenName, ItemCategory, OrdTakenDate ";
                strSql = strSql + " ORDER BY  OrdTakenDate DESC, CanteenName, ItemCategory ";
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;
                
                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    cmd.Parameters.AddWithValue("@CanteenName", objReq.CanteenName);
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    cmd.Parameters.AddWithValue("@ItemCategory", objReq.ItemCategory);
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsMonthlyReportResp();

                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i]["ItemCategory"]);
                        obj.OrderTakenDate = Convert.ToString(ds.Tables[0].Rows[i]["OrdTakenDate"]);
                        obj.TotalCoupons = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalCoupons"]);
                        obj.TotalPrice = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalPrice"]);
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
                Logger.WriteLog("Function Name : Fn_CanteenWise_Report", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                
                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_CanteenWise_Report");
            return objResp;
        }

        public List<clsMonthlyReportResp> Fn_EmployeeWise_Report(clsMonthlyReportReq objReq)
        {
            var objResp = new List<clsMonthlyReportResp>();
            var obj = new clsMonthlyReportResp();
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Request", "Fn_EmployeeWise_Report");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT EmployeeId, EmpName, CanteenId, CanteenName, ItemCategory, OrdTakenDate, COUNT(CouponId) AS TotalCoupons,  ";
                strSql = strSql + " SUM(Price) AS TotalPrice FROM vCouponOrder WHERE 1=1 AND OrdStatus = 'Scanned' ";

                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    strSql = strSql + " AND CanteenId = @CanteenId ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    strSql = strSql + " AND CanteenName = @CanteenName ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    strSql = strSql + " AND ItemCategory = @ItemCategory ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    strSql = strSql + " AND EmpLocation = @EmpLocation ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND OrdTakenDate='" + objReq.OrderTakenDate + "'";
                }
                if (!String.IsNullOrWhiteSpace(objReq.FromDate) && !String.IsNullOrWhiteSpace(objReq.ToDate))
                {
                    strSql = strSql + " AND OrdTakenDate BETWEEN '" + objReq.FromDate + "' AND '" + objReq.ToDate + "'";
                }                
                strSql = strSql + " GROUP BY EmployeeId, EmpName, CanteenId, CanteenName, ItemCategory, OrdTakenDate ";
                strSql = strSql + " ORDER BY EmpName, CanteenName, ItemCategory , OrdTakenDate DESC ";
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;

                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    cmd.Parameters.AddWithValue("@CanteenName", objReq.CanteenName);
                }
                if (!String.IsNullOrWhiteSpace(objReq.ItemCategory))
                {
                    cmd.Parameters.AddWithValue("@ItemCategory", objReq.ItemCategory);
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsMonthlyReportResp();

                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeId"]);
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        obj.ItemCategory = Convert.ToString(ds.Tables[0].Rows[i]["ItemCategory"]);
                        obj.OrderTakenDate = Convert.ToString(ds.Tables[0].Rows[i]["OrdTakenDate"]);
                        obj.TotalCoupons = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalCoupons"]);
                        obj.TotalPrice = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalPrice"]);
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
                Logger.WriteLog("Function Name : Fn_EmployeeWise_Report", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));

                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_EmployeeWise_Report");
            return objResp;
        }

        public List<clsMonthlyReportResp> Fn_CanteenWise_Summery(clsMonthlyReportReq objReq)
        {
            var objResp = new List<clsMonthlyReportResp>();
            var obj = new clsMonthlyReportResp();
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Request", "Fn_CanteenWise_Summery");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT CanteenId, CanteenName, COUNT(CouponId) AS TotalCoupons, ";
                strSql = strSql + " SUM(Price) AS TotalPrice FROM vCouponOrder WHERE 1=1 AND OrdStatus = 'Scanned' ";

                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    strSql = strSql + " AND CanteenId = @CanteenId ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    strSql = strSql + " AND CanteenName = @CanteenName ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    strSql = strSql + " AND EmpLocation = @EmpLocation ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND OrdTakenDate='" + objReq.OrderTakenDate + "'";
                }
                if (!String.IsNullOrWhiteSpace(objReq.FromDate) && !String.IsNullOrWhiteSpace(objReq.ToDate))
                {
                    strSql = strSql + " AND OrderDate BETWEEN '" + objReq.FromDate + "' AND '" + objReq.ToDate + "'";
                }
                strSql = strSql + " GROUP BY CanteenId, CanteenName ORDER BY  CanteenName ";
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;
                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    cmd.Parameters.AddWithValue("@CanteenName", objReq.CanteenName);
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsMonthlyReportResp();
                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.TotalCoupons = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalCoupons"]);
                        obj.TotalPrice = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalPrice"]);
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
                Logger.WriteLog("Function Name : Fn_CanteenWise_Summery", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));

                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_CanteenWise_Summery");
            return objResp;
        }

        public List<clsMonthlyReportResp> Fn_EmployeeWise_Summery(clsMonthlyReportReq objReq)
        {
            var objResp = new List<clsMonthlyReportResp>();
            var obj = new clsMonthlyReportResp();
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Request", "Fn_EmployeeWise_Summery");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string strSql = "SELECT CanteenId, CanteenName, EmployeeId, EmpName, COUNT(CouponId) AS TotalCoupons, ";
                strSql = strSql + " SUM(Price) AS TotalPrice FROM vCouponOrder WHERE 1=1 AND OrdStatus = 'Scanned' ";

                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    strSql = strSql + " AND CanteenId = @CanteenId ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    strSql = strSql + " AND CanteenName = @CanteenName ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    strSql = strSql + " AND EmpLocation = @EmpLocation ";
                }
                if (!String.IsNullOrWhiteSpace(objReq.OrderTakenDate))
                {
                    strSql = strSql + " AND OrdTakenDate='" + objReq.OrderTakenDate + "'";
                }
                if (!String.IsNullOrWhiteSpace(objReq.FromDate) && !String.IsNullOrWhiteSpace(objReq.ToDate))
                {
                    strSql = strSql + " AND OrderDate BETWEEN '" + objReq.FromDate + "' AND '" + objReq.ToDate + "'";
                }
                strSql = strSql + " GROUP BY CanteenId, CanteenName, EmployeeId, EmpName ORDER BY EmpName, CanteenName ";
                
                SqlCommand cmd = new SqlCommand(strSql, Con);
                cmd.CommandType = CommandType.Text;
                if (objReq.CanteenId != 0 && objReq.CanteenId != null)
                {
                    cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                }
                if (!String.IsNullOrWhiteSpace(objReq.CanteenName))
                {
                    cmd.Parameters.AddWithValue("@CanteenName", objReq.CanteenName);
                }
                if (!String.IsNullOrWhiteSpace(objReq.EmpLocation))
                {
                    cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsMonthlyReportResp();

                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        obj.EmpId = Convert.ToInt32(ds.Tables[0].Rows[i]["EmployeeId"]);
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        obj.TotalCoupons = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalCoupons"]);
                        obj.TotalPrice = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalPrice"]);
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
                Logger.WriteLog("Function Name : Fn_EmployeeWise_Summery", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));

                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_EmployeeWise_Summery");
            return objResp;
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
    }
}