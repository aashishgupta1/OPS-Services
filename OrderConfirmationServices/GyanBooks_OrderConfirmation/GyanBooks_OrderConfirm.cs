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
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GyanBooks_OrderConfirmation
{
    public class GyanBooks_OrderConfirm : ModuleBase
    {
        string marketPlaceID = string.Empty;
        string userName = string.Empty;
        string password = string.Empty;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        const string POSTURL = "https://www.gyanbooks.com/order/api/orderupdateapicurl.php?ordercode={0}&status={1}&tn=3";
        const string ORDERITEMPOSTURL = "https://www.gyanbooks.com/order/api/orderupdateapicurl.php?ordercode={0}&bookcode={1}&status={2}&tn=2";
        public GyanBooks_OrderConfirm(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            GetGyanBooksDetails();
        }

        private bool GetGyanBooksDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                marketPlaceID = Convert.ToString(data.Rows[0]["ID"]);
                userName = Convert.ToString(data.Rows[0]["userName"]);
                password = Convert.ToString(data.Rows[0]["Password"]);
            }
            return result;
        }
        public override void Run()
        {
            try
            {
                if (!OrderUpdateRequest())
                {
                    logger.LogError("Error in updating record");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }
        private bool OrderUpdateRequest()
        {
            bool bResult = false;
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
                bool updateOrderStatus = false;
                if (string.IsNullOrEmpty(Convert.ToString(dataRows[0]["OrderItemID"])))
                {
                    updateOrderStatus = UpdateCompleteOrder(orderID, Convert.ToString(dataRows[0]["TrackingNumber"]), Convert.ToString(dataRows[0]["UpdateStatus"]), Convert.ToString(dataRows[0]["LogisticName"]), ref errorCode, ref errorMessage);
                }
                else
                {
                    updateOrderStatus = updateSpecificItem(orderID, dataRows, ref errorCode, ref errorMessage);
                }
                UpdateDBStatus(orderID, updateOrderStatus, errorCode, errorMessage);
            }
            return bResult;
        }


        private bool UpdateCompleteOrder(string orderID, string trackingNumber, string updateStatus, string logisticName, ref string sErrroCode, ref string sErrorMessage)
        {
            bool bResult = false;
            HttpResponseMessage httpResponseMessage = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ModuleSetings.PostURL);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.ASCIIEncoding.ASCII.GetBytes(
                   $"{userName}:{password}")));
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string finalURL = string.Format(POSTURL, orderID, updateStatus);
                httpResponseMessage = httpClient.GetAsync(POSTURL).Result;
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {                   
                    bResult = true;
                }
                else
                {
                    logger.LogError("Issue in updating Order : " + httpResponseMessage.StatusCode + ":" + httpResponseMessage.Content.ToString());
                }
            }
            return bResult;
        }
        private bool updateSpecificItem(string orderID, List<DataRow> dataRows, ref string sErrroCode, ref string sErrorMessage)
        {
            bool bResult = false;
            HttpResponseMessage httpResponseMessage = null;
            foreach (DataRow dataRow in dataRows)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(ModuleSetings.PostURL);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                       $"{userName}:{password}")));
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string finalURL = string.Format(ORDERITEMPOSTURL, orderID, Convert.ToString(dataRows[0]["OrderItemID"]), Convert.ToString(dataRows[0]["UpdateStatus"]));
                    httpResponseMessage = httpClient.GetAsync(POSTURL).Result;
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        bResult = true;
                    }
                    else
                    {
                        logger.LogError("Issue in updating Order : " + httpResponseMessage.StatusCode + ":" + httpResponseMessage.Content.ToString());
                    }
                }
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
