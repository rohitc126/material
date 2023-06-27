using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DAL
{
    public class DAL_MATERIAL_REQUISITION_ENTRY
    {
        public string INSERT_MATERIAL_REQUISITION_ENTRY(string Emp_Code, Material_Entry MAT, out ResultMessage oblMsg)
        {
            string errorMsg = "";
            oblMsg = new ResultMessage();
         
            using (var connection = new SqlConnection(sqlConnection.GetConnectionString_SGX()))
            {
                connection.Open();
                SqlCommand command;
                SqlTransaction transactionScope = null;
                transactionScope = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {

                    SqlParameter[] param =
                                    {
                                       new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200) 
                                       ,new SqlParameter("@REQUISITION_ID", SqlDbType.Decimal) 
                                      ,new SqlParameter("@REQUISITION_CODE", SqlDbType.VarChar, 20)
                                       
                                      ,new SqlParameter("@REQUISITION_DATE", MAT.Requisition_DT)
                                      ,new SqlParameter("@BRANCH_CODE",MAT.BRANCH_CODE)  
                                      ,new SqlParameter("@MATERIAL_ID",MAT.Material_Id)             
                                      ,new SqlParameter("@QUANTITY",  MAT.Qty) 
                                      ,new SqlParameter("@UNIT", MAT.Unit)
                                      ,new SqlParameter("@EXPECTED_DEL_DATE",MAT.Expected_DT)
                                      ,new SqlParameter("@ADDED_BY",Emp_Code)  
                    
                    };

                    param[0].Direction = ParameterDirection.Output;
                    param[1].Direction = ParameterDirection.Output;
                    param[2].Direction = ParameterDirection.Output;

                    new DataAccess().InsertWithTransaction("[AGG].[USP_INSERT_MATERIAL_REQUISITION_ENTRY]", CommandType.StoredProcedure, out command, connection, transactionScope, param);
                    decimal REQUISITION_ID = (decimal)command.Parameters["@REQUISITION_ID"].Value;
                    string REQUISITION_CODE = (string)command.Parameters["@REQUISITION_CODE"].Value;
                    string error_1 = (string)command.Parameters["@ERRORSTR"].Value;
                    if (REQUISITION_ID == -1) { errorMsg = error_1; }



                    if (errorMsg == "")
                    {

                        oblMsg.ID = REQUISITION_ID;
                        oblMsg.CODE = REQUISITION_CODE;
                        oblMsg.MsgType = "Success";
                        transactionScope.Commit();

                    }

                    else
                    {
                        oblMsg.Msg = errorMsg;
                        oblMsg.MsgType = "Error";
                        transactionScope.Rollback();
                    }
                }

                catch (Exception ex)
                {

                    errorMsg = "Error: Exception occured. " + ex.StackTrace.ToString();

                }
                finally
                {
                    connection.Close();
                }
            }
            return errorMsg;
        }


        public List<MATERIAL_REQ_DATA_LIST> Select_Material_Data_List(Material_Req_List materiallist)
        {
            SqlParameter[] param = {  
                                      
                                       new SqlParameter("@BRANCH_CODE", materiallist.BRANCH_CODE),
                                       new SqlParameter("@FROM_DT", materiallist.From_DT),
                                       new SqlParameter("@TO_DT", materiallist.To_DT) 
                                   };

            DataSet ds = new DataAccess(sqlConnection.GetConnectionString_SGX()).GetDataSet("[AGG].[USP_SELECT_MATERIAL_REQUISITION_LIST]", CommandType.StoredProcedure, param);

            List<MATERIAL_REQ_DATA_LIST> _list = new List<MATERIAL_REQ_DATA_LIST>();
            DataTable dt = ds.Tables[0];
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    _list.Add(new MATERIAL_REQ_DATA_LIST
                    {
                        REQUISITION_ID = Convert.ToDecimal(row["REQUISITION_ID"]),
                        REQUISITION_CODE = Convert.ToString(row["REQUISITION_CODE"]),
                        REQUISITION_DATE = Convert.ToString(row["REQUISITION_DATE"]),
                        Material_Name = Convert.ToString(row["Material_Name"]),
                        QUANTITY = Convert.ToDecimal(row["QUANTITY"]),
                        UNIT = Convert.ToString(row["UNIT"]),
                        EXPECTED_DEL_DATE = Convert.ToString(row["EXPECTED_DEL_DATE"]),
                    });
                }
            }

            return _list;
        }


     

    }
}
