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
                else if (objReq.CanteenId == null || objReq.CanteenId == "0")
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
                        objItem.CanteenId = Convert.ToString(ds.Tables[0].Rows[i]["CantId"]);
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



    }
}