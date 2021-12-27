using DBInteraction;
using ModuleCore;
using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Biblio_OrderConfirmation
{
    class Biblio_OrderConfirm : ModuleBase
    {
        private string marketPlaceID = string.Empty;
        private string userName = string.Empty;
        private string password = string.Empty;
        private bool autoUpdate = false;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public Biblio_OrderConfirm(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
        }

        
        public override void Run()
        {
            if (GetBiblioDetails())
            {
                if (autoUpdate)
                {
                    CreateOrderUpdateFile();
                }
            }
        }

        private bool GetBiblioDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                marketPlaceID = Convert.ToString(data.Rows[0]["ID"]);
                userName = Convert.ToString(data.Rows[0]["userName"]);
                password = Convert.ToString(data.Rows[0]["Password"]);
                autoUpdate = Convert.ToBoolean(data.Rows[0]["AutoUpdate"]);
                result = true;
            }
            return result;
        }

        private bool CreateOrderUpdateFile()
        {
            bool bResult = false;
            DataTable updateRecord = new DataTable();
            #region Get All Order
            updateRecord = GetAllOrderForUpdate();
            #endregion
            string sFileName = CreateConfirmFile(updateRecord);
            UploadFile(sFileName);
            foreach(DataRow data in updateRecord.Rows)
            {
                UpdateDBStatus(Convert.ToString(data["OrderID"]), true, "", "");
            }

            return bResult;

        }

        private void UploadFile(string fileName)
        {
            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(userName, password);
                client.UploadFile(ModuleSetings.FTPDetails.FtpHost, WebRequestMethods.Ftp.UploadFile, fileName);
            }
        }
        private bool UpdateDBStatus(string orderID, bool isSuccess, string errorCode, string errorMessage)
        {
            DataTable dt = new DataTable();

            bool bResult = false;
            DBOperation.StructDBOperation[] structDBOperations = new DBOperation.StructDBOperation[4];
            int iParam = 0;
            structDBOperations[iParam] = new DBOperation.StructDBOperation();
            structDBOperations[iParam].sParamName = "@vOrderID";
            structDBOperations[iParam].sParamType = SqlDbType.VarChar;
            structDBOperations[iParam].sParamValue = orderID;

            iParam++;
            structDBOperations[iParam] = new DBOperation.StructDBOperation();
            structDBOperations[iParam].sParamName = "@vIsSuccess";
            structDBOperations[iParam].sParamType = SqlDbType.VarChar;
            structDBOperations[iParam].sParamValue = isSuccess ? "1" : "0";

            iParam++;
            structDBOperations[iParam] = new DBOperation.StructDBOperation();
            structDBOperations[iParam].sParamName = "@vErrorCode";
            structDBOperations[iParam].sParamType = SqlDbType.VarChar;
            structDBOperations[iParam].sParamValue = errorCode;

            iParam++;
            structDBOperations[iParam] = new DBOperation.StructDBOperation();
            structDBOperations[iParam].sParamName = "@vErrorMessage";
            structDBOperations[iParam].sParamType = SqlDbType.VarChar;
            structDBOperations[iParam].sParamValue = errorMessage;


            string sRetval = DBOperation.ExecuteDBOperation("sp_Update_MarketPlaceUpdateStatus", DBOperation.OperationType.STOREDPROC_UPDATE, structDBOperations, ref dt);
            if (sRetval != SUCCESS)
            {
                logger.LogError("Error in updating DB Status for order ID :" + orderID + " \nReason : " + sRetval);
            }
            return bResult;
        }
        private string CreateConfirmFile(DataTable updateRecord)
        {
            string assemblyFile = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            string FileName = Path.Combine(assemblyFile, DateTime.Now.ToString("ddMMyyyyhhmmsss") + ".txt");
            StreamWriter sw = new StreamWriter(FileName);
            sw.WriteLine("order_id\torder_item_id\tstatus\tquantity\tcarrier\ttracking_number\trefund_reason\trefund_type\trefund_amount\treturn_postage_amount");
            foreach(DataRow dr in updateRecord.Rows)
            {
                sw.WriteLine(dr["OrderID"] + "\t" + dr["OrderItemID"] + "\t" + dr["UpdateStatus"] + "\t1\t" + dr["LogisticName"] + "\t" + dr["TrackingNumber"]+ "\t\t\t\t");
            }
            sw.Close();
            return FileName;
        }
        private DataTable GetAllOrderForUpdate()
        {
            DataTable dt = new DataTable();
            string sRetVal = string.Empty;
            DBOperation.StructDBOperation[] structDBOperation = new DBOperation.StructDBOperation[1];
            int iParam = 0;
            structDBOperation[iParam] = new DBOperation.StructDBOperation();
            structDBOperation[iParam].sParamName = "@vMarketPlaceID";
            structDBOperation[iParam].sParamType = SqlDbType.VarChar;
            structDBOperation[iParam].sParamValue = marketPlaceID;

            sRetVal = DBOperation.ExecuteDBOperation("sp_GetRecordForMarketPlaceUpdate", DBOperation.OperationType.STOREDPROC, structDBOperation, ref dt);
            if (sRetVal != SUCCESS)
            {
                logger.LogError("Error in fetching details:" + sRetVal);
            }
            return dt;
        }

    }
}
