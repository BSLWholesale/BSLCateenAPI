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
    public class DALEmployee
    {
        SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["BSL"].ConnectionString);


        public clsEmployee Fn_Login_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Login_Employee");
            try
            {
                if (objReq.EmpId == null || objReq.EmpId == 0) 
                {
                    objResp.vErrorMsg = "Please Enter Employee ID";
                }
                if (objReq.EmpMobile == null || objReq.EmpMobile == "0")
                {
                    objResp.vErrorMsg = "Please Enter Employee ID";
                }
                if (objReq.LoginID == null || objReq.LoginID == "0")
                {
                    objResp.vErrorMsg = "Please Enter Employee ID";
                }
                if (String.IsNullOrWhiteSpace(objReq.EmpPassword))
                {
                    objResp.vErrorMsg = "Please Enter the Password";
                }
                else
                {
                    if (Con.State == ConnectionState.Broken)
                    { Con.Close(); }
                    if (Con.State == ConnectionState.Closed)
                    { Con.Open(); }

                    string encryptPassword = Generic.EncryptText(objReq.EmpPassword);

                    SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60; // Optional
                    if (objReq.LoginID == null)
                    {
                        cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                        cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);
                        cmd.Parameters.AddWithValue("@EmpPassword", encryptPassword);
                        cmd.Parameters.AddWithValue("@QueryType", "Login");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@LoginID", objReq.LoginID);
                        cmd.Parameters.AddWithValue("@EmpPassword", encryptPassword);
                        cmd.Parameters.AddWithValue("@QueryType", "LoginStaff");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int i = 0;

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        objResp.EmpId = Convert.ToInt64(ds.Tables[0].Rows[i]["EmployeeId"]);
                        objResp.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        objResp.EmpMobile = Convert.ToString(ds.Tables[0].Rows[i]["EmpMobile"]);
                        objResp.Department = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);

                        //if (ds.Tables[0].Rows[i]["EmpMobile"] == DBNull.Value)
                        //{
                        //    objResp.EmpMobile = string.Empty;
                        //}
                        //else
                        //{
                        //    objResp.EmpMobile = Convert.ToString(ds.Tables[0].Rows[i]["EmpMobile"]);
                        //}

                        //if (ds.Tables[0].Rows[i]["Department"] == DBNull.Value)
                        //{
                        //    objResp.Department = string.Empty;
                        //}
                        //else
                        //{
                        //    objResp.Department = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);
                        //}

                        objResp.EmpLocation = Convert.ToString(ds.Tables[0].Rows[i]["EmpLocation"]);
                        objResp.EmpRole = Convert.ToString(ds.Tables[0].Rows[i]["EmpRole"]);
                        if (objResp.EmpRole != "Admin" && objResp.EmpRole != "Staff" && objResp.EmpRole != "Supervisor")
                        {
                            objResp.vErrorMsg = "You are not a valid employee to access the portal.";
                            return objResp;
                        }

                        objResp.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        objResp.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);

                        //if (ds.Tables[0].Rows[i]["LoginID"] == DBNull.Value)
                        //{
                        //    objResp.LoginID = string.Empty;
                        //}
                        //else
                        //{
                        //    objResp.LoginID = Convert.ToString(ds.Tables[0].Rows[i]["LoginID"]);
                        //}
                        objResp.LoginID = Convert.ToString(ds.Tables[0].Rows[i]["LoginID"]);
                        objResp.EmpStatus = Convert.ToBoolean(ds.Tables[0].Rows[i]["EmpStatus"]);

                        objResp.vErrorMsg = "Success";
                    }
                    else
                    {
                        objResp.vErrorMsg = "Entered Credentials has been invalid";
                    }
                }                          
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Login_Employee", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Login_Employee");
            return objResp;
        }


        public clsEmployee Fn_Insert_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Insert_Employee");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string EncryptPassword = "";
                if (!String.IsNullOrWhiteSpace(objReq.EmpPassword))
                {
                    EncryptPassword = Generic.EncryptText(objReq.EmpPassword);
                }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@EmpName", objReq.EmpName);
                cmd.Parameters.AddWithValue("@Department", objReq.Department);
                cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);
                cmd.Parameters.AddWithValue("@EmpPassword", EncryptPassword);
                cmd.Parameters.AddWithValue("@EmpRole", objReq.EmpRole);
                cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                cmd.Parameters.AddWithValue("@CreatedBy", objReq.CreatedBy);
                cmd.Parameters.AddWithValue("@QueryType", "InsertEmployee");

                int i = 0;
                i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    objResp.vErrorCode = 200;
                    objResp.vErrorMsg = "Success";
                }
                else
                {
                    objResp.vErrorCode = 400;
                    objResp.vErrorMsg = "Employee/Worker Insertion failed";
                }
            }
            catch (Exception exp)
            {
                objResp.vErrorCode = 500;
                Logger.WriteLog("Function Name : Fn_Insert_Employee", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Insert_Employee");
            return objResp;
        }


        public List<clsEmployee> Fn_Fetch_EmployeeDetails(clsEmployee objReq)
        {
            var objResp = new List<clsEmployee>();
            var obj = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Fetch_EmployeeDetails");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@QueryType", "FetchEmpDetails");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        obj = new clsEmployee();
                        obj.EmpId = Convert.ToInt64(ds.Tables[0].Rows[i]["EmployeeId"]);
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        
                        if (ds.Tables[0].Rows[i]["EmpMobile"] == DBNull.Value)
                        {
                            obj.EmpMobile = string.Empty;
                        }
                        else
                        {
                            obj.EmpMobile = Convert.ToString(ds.Tables[0].Rows[i]["EmpMobile"]);
                        }

                        if (ds.Tables[0].Rows[i]["Department"] == DBNull.Value)
                        {
                            obj.Department = string.Empty;
                        }
                        else
                        {
                            obj.Department = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);
                        }

                        obj.EmpLocation = Convert.ToString(ds.Tables[0].Rows[i]["EmpLocation"]);
                        obj.CanteenId = Convert.ToInt32(ds.Tables[0].Rows[i]["CanteenId"]);
                        obj.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        
                        if (ds.Tables[0].Rows[i]["EmpRole"] == DBNull.Value)
                        {
                            obj.EmpRole = string.Empty;
                        }
                        else
                        {
                            obj.EmpRole = Convert.ToString(ds.Tables[0].Rows[i]["EmpRole"]);
                        }

                        string strEmpPassword = Convert.ToString(ds.Tables[0].Rows[i]["EmpPassword"]);
                        string decryptPassword = "";
                        if (!String.IsNullOrWhiteSpace(strEmpPassword))
                        {
                            decryptPassword = Generic.DecryptText(Convert.ToString(ds.Tables[0].Rows[i]["EmpPassword"]));
                            obj.EmpPassword = decryptPassword;
                        }

                        obj.EmpStatus = Convert.ToBoolean(ds.Tables[0].Rows[i]["EmpStatus"]);

                        obj.vErrorMsg = "Success";
                        obj.vErrorCode = 200;
                        objResp.Add(obj);
                        i++;
                    }
                }
                else
                {
                    obj.vErrorMsg = "Employee details are not found.";
                    obj.vErrorCode = 400;
                }

            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Fetch_EmployeeDetails", " " + exp.Message.ToString(), new StackTrace(exp, true));
                obj.vErrorMsg = exp.Message.ToString();
                obj.vErrorCode = 500;
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Fetch_EmployeeDetails");
            return objResp;
        }


        public clsEmployee Fn_Update_EmployeeDetails(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Update_EmployeeDetails");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                string EncryptPassword = Generic.EncryptText(objReq.EmpPassword);

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@EmpName", objReq.EmpName);
                cmd.Parameters.AddWithValue("@Department", objReq.Department);
                cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);                
                cmd.Parameters.AddWithValue("@EmpPassword", EncryptPassword);
                cmd.Parameters.AddWithValue("@EmpRole", objReq.EmpRole);
                cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                cmd.Parameters.AddWithValue("@CanteenId", objReq.CanteenId);
                cmd.Parameters.AddWithValue("@EmpStatus", objReq.EmpStatus);
                cmd.Parameters.AddWithValue("@ModifiedBy", objReq.ModifiedBy);
                cmd.Parameters.AddWithValue("@QueryType", "UpdateEmpDetails");

                int i = 0;
                i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    objResp.vErrorMsg = "Success";
                    objResp.vErrorCode = 200;
                }
                else
                {
                    objResp.vErrorMsg = "Employee updation is failed";
                    objResp.vErrorCode = 400;
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Update_EmployeeDetails", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Update_EmployeeDetails");
            return objResp;
        }


        public clsEmployee Fn_Delete_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Delete_Employee");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@QueryType", "DeleteEmployee");

                int i = 0;
                i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    objResp.vErrorMsg = "Success";
                    objResp.vErrorCode = 200;
                }
                else
                {
                    objResp.vErrorMsg = "Employee Deletion failed";
                    objResp.vErrorCode = 400;
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Delete_Employee", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
                objResp.vErrorCode = 500;
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Delete_Employee");
            return objResp;
        }



        public List<clsEmployee> Fn_Fetch_All_Employee(clsEmployee objReq)
        {
            var objResp = new List<clsEmployee>();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Fetch_All_Employee");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                cmd.Parameters.AddWithValue("@QueryType", "FetchAllEmployee");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var objItem = new clsEmployee();
                        objItem.EmpId = Convert.ToInt64(ds.Tables[0].Rows[i]["EmployeeId"]);
                        objItem.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["EmpName"]);
                        
                        if (ds.Tables[0].Rows[i]["Department"] == DBNull.Value)
                        {
                            objItem.Department = string.Empty;
                        }
                        else
                        {
                            objItem.Department = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);
                        }                       

                        if (ds.Tables[0].Rows[i]["EmpMobile"] == DBNull.Value)
                        {
                            objItem.EmpMobile = string.Empty;
                        }
                        else
                        {
                            objItem.EmpMobile = Convert.ToString(ds.Tables[0].Rows[i]["EmpMobile"]);
                        }
                        
                        objItem.EmpLocation = Convert.ToString(ds.Tables[0].Rows[i]["EmpLocation"]);

                        if (ds.Tables[0].Rows[i]["EmpRole"] == DBNull.Value)
                        {
                            objItem.EmpMobile = string.Empty;
                        }
                        else
                        {
                            objItem.EmpRole = Convert.ToString(ds.Tables[0].Rows[i]["EmpRole"]);
                        }

                        objItem.CanteenName = Convert.ToString(ds.Tables[0].Rows[i]["CanteenName"]);
                        objItem.vErrorMsg = "Success";
                        objResp.Add(objItem);
                        i++;
                    }
                }
                else
                {
                    var objItem = new clsEmployee();
                    objItem.vErrorMsg = "No Employee Records found.";
                    objResp.Add(objItem);
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Fetch_All_Employee", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                var objItem = new clsEmployee();
                objItem.vErrorMsg = exp.Message.ToString();
                objResp.Add(objItem);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Fetch_All_Employee");
            return objResp;
        }

        public List<clsResponseDropdown> Fn_Fill_DropdownList(clsRequestDropdown objReq)
        {
            var objResp = new List<clsResponseDropdown>();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Fill_DropdownList");
            try
            {
                string strSql = "";
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                if (String.IsNullOrWhiteSpace(objReq.vValueField))
                {
                    strSql = "select Distinct " + objReq.vFieldName + " from " + objReq.vTBLName + " where 1=1";
                    if (!String.IsNullOrWhiteSpace(objReq.vCriteria))
                    {
                        strSql = strSql + objReq.vCriteria;
                    }
                    strSql = strSql + " order by " + objReq.vFieldName + "";
                }
                else
                {
                    strSql = "select Distinct " + objReq.vValueField + ", " + objReq.vFieldName + " from " + objReq.vTBLName + " where 1=1";
                    if (!String.IsNullOrWhiteSpace(objReq.vCriteria))
                    {
                        strSql = strSql + objReq.vCriteria;
                    }
                    strSql = strSql + " order by " + objReq.vValueField + "";
                }

                SqlDataAdapter da = new SqlDataAdapter(strSql, Con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var obj = new clsResponseDropdown();

                        if (String.IsNullOrWhiteSpace(objReq.vValueField))
                        {
                            obj.vFieldName = Convert.ToString(ds.Tables[0].Rows[i][0]);
                        }
                        else
                        {
                            obj.vValueField = Convert.ToString(ds.Tables[0].Rows[i][0]);
                            obj.vFieldName = Convert.ToString(ds.Tables[0].Rows[i][1]);
                        }

                        obj.vErrorMsg = "Success";
                        objResp.Add(obj);
                        i++;
                    }
                }
                else
                {

                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Fill_DropdownList", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                var obj = new clsResponseDropdown();
                obj.vErrorMsg = exp.Message.ToString();
                objResp.Add(obj);
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Fill_DropdownList");
            return objResp;
        }


        public clsEmployee Fn_Upload_BulkEmployeeData_List(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Upload_BulkEmployeeData_List");
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_BulkEmpDataImport", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@EmpName", objReq.EmpName);
                cmd.Parameters.AddWithValue("@EmpLocation", objReq.EmpLocation);
                cmd.Parameters.AddWithValue("@Department", objReq.Department);
                cmd.Parameters.AddWithValue("@EmpRole", objReq.EmpRole);
                cmd.Parameters.AddWithValue("@CateenId", objReq.CanteenId);
                cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);

                int i = 0;
                i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    objResp.vErrorMsg = "Success";
                }
                else
                {
                    objResp.vErrorMsg = "Failed";
                }                
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Upload_BulkEmployeeData_List", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Upload_BulkEmployeeData_List");
            return objResp;
        }


        public clsEmployee Fn_Reset_Password(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            Logger.ErrorLog(JsonConvert.SerializeObject(objReq), "Request", "Fn_Reset_Password");
            try
            {
                if (objReq.EmpId == null || objReq.EmpId == 0)
                {
                    objResp.vErrorMsg = "Please Enter Employee ID";
                }
                else if (String.IsNullOrWhiteSpace(objReq.EmpPassword))
                {
                    objResp.vErrorMsg = "Please Enter Password";
                }
                else
                {
                    if (Con.State == ConnectionState.Broken)
                    { Con.Close(); }
                    if (Con.State == ConnectionState.Closed)
                    { Con.Open(); }

                    string EncryptPassword = Generic.EncryptText(objReq.EmpPassword);
                    SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                    cmd.Parameters.AddWithValue("@EmpPassword", EncryptPassword);
                    cmd.Parameters.AddWithValue("@QueryType", "ResetPassword");

                    int i = 0;
                    i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        objResp.vErrorMsg = "Success";
                    }
                    else
                    {
                        objResp.vErrorMsg = "Failed";
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLog("Function Name : Fn_Reset_Password", " " + "Error Msg : " + exp.Message.ToString(), new StackTrace(exp, true));
                objResp.vErrorMsg = exp.Message.ToString();
            }
            finally
            {
                Con.Close();
            }
            Logger.ErrorLog(JsonConvert.SerializeObject(objResp), "Response", "Fn_Reset_Password");
            return objResp;
        }



    }
}