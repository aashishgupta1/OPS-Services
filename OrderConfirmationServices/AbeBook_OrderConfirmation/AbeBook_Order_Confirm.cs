using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AbeBook_OrderConfirmation
{
    public class AbeBook_Order_Confirm : ModuleBase
    {
        private string allOrderUpdateRequest = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><orderUpdateRequest version = \"1.0\" ><action name=\"update\"> " +
            "<username>{0}</username><password>{1}</password></action>" +
            "<purchaseOrder id = \"{2}\"><shipping> " +
            "<company>{3}</company><trackingCode>{4}</trackingCode>" +
            "</shipping><status>{5}</status>" +
            "</purchaseOrder></orderUpdateRequest>";

        private string specificOrderUpdateRequest = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><orderUpdateRequest version = \"1.0\" ><action name=\"update\"> " +
            "<username>{0}</username><password>{1}</password></action>" +
            "<purchaseOrder id = \"{2}\">" +
            "<purchaseOrderItemList>{3}</purchaseOrderItemList>" +
        "</purchaseOrder></orderUpdateRequest>";

        private string specificOrderItemDetails = "<purchaseOrderItem id=\"{0}\"><status>{1}</status>" +
       "</purchaseOrderItem>";


        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        string userName = string.Empty;
        string password = string.Empty;
        string marketPlaceID = string.Empty;

        public AbeBook_Order_Confirm(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            
        }
        private bool GetAbeBokDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                marketPlaceID = Convert.ToString(data.Rows[0]["ID"]);
                userName = Convert.ToString(data.Rows[0]["userName"]);
                password = Convert.ToString(data.Rows[0]["Password"]);
                result = true;
            }
            return result;
        }
        public override void Run()
        {
            try
            {
                if (GetAbeBokDetails())
                {
                    if (!OrderUpdateRequest())
                    {
                        logger.LogError("Error in updating record");
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e);
            }
            
        }
        private bool OrderUpdateRequest()
        {
            bool bResult = true;
            DataTable updateRecord = new DataTable();
            #region Get All Order
            updateRecord = GetAllOrderForUpdate();
            #endregion

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var tableAsList = updateRecord.AsEnumerable();
            List<string> val = tableAsList.Select(x => x.Field<string>("OrderID")).Distinct().ToList();
            string errorCode = string.Empty;
            string errorMessage = string.Empty;
            foreach (string orderID in val)
            {
                List<DataRow> dataRows = tableAsList.Where(x => x.Field<string>("OrderID").Equals(orderID)).ToList();
                bool updateAbeStatus = false;
                if (string.IsNullOrEmpty(Convert.ToString(dataRows[0]["OrderItemID"])))
                {
                    updateAbeStatus = UpdateCompleteOrder(orderID, Convert.ToString(dataRows[0]["TrackingNumber"]), Convert.ToString(dataRows[0]["UpdateStatus"]), Convert.ToString(dataRows[0]["LogisticName"]), ref errorCode, ref errorMessage);
                }
                else
                {
                    updateAbeStatus = updateSpecificItem(orderID, dataRows, ref errorCode, ref errorMessage);
                }
                UpdateDBStatus(orderID, updateAbeStatus, errorCode, errorMessage);
            }
            return bResult;
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
            structDBOperations[iParam].sParamValue = isSuccess ? "1":"0";

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


            string sRetval =  DBOperation.ExecuteDBOperation("sp_Update_MarketPlaceUpdateStatus", DBOperation.OperationType.STOREDPROC_UPDATE, structDBOperations, ref dt);
            if(sRetval != SUCCESS)
            {
                logger.LogError("Error in updating DB Status for order ID :" + orderID + " \nReason : " + sRetval);
            }
            return bResult;
        }
        private bool UpdateCompleteOrder(string orderID, string trackingNumber, string updateStatus, string logisticName, ref string sErrroCode, ref string sErrorMessage)
        {
            bool bResult = false;
            HttpClient httpClient = new HttpClient();
            string requestXML = string.Format(allOrderUpdateRequest, userName, password, orderID, logisticName, trackingNumber, updateStatus);
            StringContent content = new StringContent(requestXML, Encoding.UTF8, "text/xml");
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                responseMessage = httpClient.PostAsync(new Uri(ModuleSetings.PostURL), content).GetAwaiter().GetResult();
            }
            catch (Exception e1)
            {
                logger.LogError(e1.Message);
            }
            if (responseMessage.IsSuccessStatusCode)
            {
                bResult = true;
            }
            else
            {
                sErrroCode = "Error";
                sErrorMessage = "Error";
                logger.LogError("Error in updating Details on AbeBook with error : " + responseMessage.Content.ReadAsStringAsync().Result + "\n as input XML : " + requestXML);
            }
            return bResult;
        }
        private bool updateSpecificItem(string orderID, List<DataRow> dataRows, ref string sErrroCode, ref string sErrorMessage)
        {
            bool bResult = false;
            HttpClient httpClient = new HttpClient();
            string orderItemRequest = string.Empty;
            foreach(DataRow dataRow in dataRows)
            {
                orderItemRequest += string.Format(specificOrderItemDetails, dataRow["OrderItemID"], dataRow["UpdateStats"]);
            }
            string requestXML = string.Format(specificOrderUpdateRequest, userName, password, orderID, orderItemRequest);
            StringContent content = new StringContent(requestXML, Encoding.UTF8, "text/xml");
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                responseMessage = httpClient.PostAsync(new Uri(ModuleSetings.PostURL), content).GetAwaiter().GetResult();
            }
            catch (Exception e1)
            {
                logger.LogError(e1.Message);
            }
            if (responseMessage.IsSuccessStatusCode)
            {
                bResult = true;
            }
            else
            {
                sErrroCode = "Error";
                sErrorMessage = "Error";
                logger.LogError("Error in updating Details on AbeBook with error : " + responseMessage.Content.ReadAsStringAsync().Result + "\n as input XML : " + requestXML);
            }
            return bResult;
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
