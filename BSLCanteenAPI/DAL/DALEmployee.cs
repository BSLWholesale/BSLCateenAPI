using BSLCanteenAPI.Models;
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
            try
            {
                if (objReq.EmpId == null || objReq.EmpId == 0 && objReq.EmpMobile == null || objReq.EmpMobile == "0")
                {
                    objResp.vErrorMsg = "Please Enter Employee ID";
                }
                else if (String.IsNullOrWhiteSpace(objReq.EmpPassword))
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
                    cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                    cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);
                    cmd.Parameters.AddWithValue("@EmpPassword", encryptPassword);
                    cmd.Parameters.AddWithValue("@QueryType", "LogIn");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int i = 0;

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        objResp.EmpId = Convert.ToInt64(ds.Tables[0].Rows[i]["EmployeeId"]);
                        objResp.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                        objResp.EmpMobile = Convert.ToString(ds.Tables[0].Rows[i]["EmpMobile"]);
                        objResp.Department = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);
                        objResp.Location = Convert.ToString(ds.Tables[0].Rows[i]["Location"]);
                        objResp.EmpRole = Convert.ToString(ds.Tables[0].Rows[i]["EmpRole"]);

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
            return objResp;
        }


        public clsEmployee Fn_Insert_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);
                cmd.Parameters.AddWithValue("@Name", objReq.EmpName);
                cmd.Parameters.AddWithValue("@Department", objReq.Department);
                cmd.Parameters.AddWithValue("@Salary", objReq.Salary);
                cmd.Parameters.AddWithValue("@EmpPassword", objReq.EmpPassword);
                cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);
                cmd.Parameters.AddWithValue("@Location", objReq.Location);
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
                    objResp.vErrorMsg = "Employee Insertion failed";
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
            return objResp;
        }


        public List<clsEmployee> Fn_Fetch_EmployeeDetails(clsEmployee objReq)
        {
            var objResp = new List<clsEmployee>();
            var obj = new clsEmployee();
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
                        obj.EmpName = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                        obj.EmpMobile = Convert.ToString(ds.Tables[0].Rows[i]["EmpMobile"]);
                        obj.Department = Convert.ToString(ds.Tables[0].Rows[i]["Department"]);
                        obj.Location = Convert.ToString(ds.Tables[0].Rows[i]["Location"]);

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
            return objResp;
        }


        public clsEmployee Fn_Update_EmployeeDetails(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", objReq.EmpId);                
                cmd.Parameters.AddWithValue("@Name", objReq.EmpName);
                cmd.Parameters.AddWithValue("@Department", objReq.Department);
                cmd.Parameters.AddWithValue("@Salary", objReq.Salary);
                cmd.Parameters.AddWithValue("@EmpPassword", objReq.EmpPassword);
                cmd.Parameters.AddWithValue("@EmpMobile", objReq.EmpMobile);
                cmd.Parameters.AddWithValue("@Location", objReq.Location);
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
            return objResp;
        }


        public clsEmployee Fn_Delete_Employee(clsEmployee objReq)
        {
            var objResp = new clsEmployee();
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
            return objResp;
        }



        public List<clsEmployee> Fn_Fetch_All_Employee(clsEmployee objReq)
        {
            var objResp = new List<clsEmployee>();
            try
            {
                if (Con.State == ConnectionState.Broken)
                { Con.Close(); }
                if (Con.State == ConnectionState.Closed)
                { Con.Open(); }

                SqlCommand cmd = new SqlCommand("USP_Employee", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QueryType", "FetchEmployee");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int i = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    while (ds.Tables[0].Rows.Count > i)
                    {
                        var objItem = new clsEmployee();
                        objItem.EmpId = Convert.ToInt64(ds.Tables[0].Rows[i][0]);
                        objItem.vErrorMsg = "Success";
                        objResp.Add(objItem);
                        i++;
                    }
                }
                else
                {
                    var objItem = new clsEmployee();
                    objItem.vErrorMsg = "No Records found.";
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
            return objResp;
        }


    }
}