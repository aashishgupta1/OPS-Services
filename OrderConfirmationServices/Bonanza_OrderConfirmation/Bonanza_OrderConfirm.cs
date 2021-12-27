using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bonanza_OrderConfirmation
{
    class Bonanza_OrderConfirm : ModuleBase
    {
        public string devID = string.Empty;
        public string certID = string.Empty;
        public string AuthToken = string.Empty;
        string marketPlaceID = string.Empty;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public Bonanza_OrderConfirm(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            GetBonanzaDetails();
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
                bool updateAbeStatus = false;
                updateAbeStatus = UpdateCompleteOrder(orderID, Convert.ToString(dataRows[0]["TrackingNumber"]), Convert.ToString(dataRows[0]["UpdateStatus"]), Convert.ToString(dataRows[0]["LogisticName"]), ref errorCode, ref errorMessage);
                
                UpdateDBStatus(orderID, updateAbeStatus, errorCode, errorMessage);
            }
            return bResult;
        }
        private bool UpdateCompleteOrder(string orderID, string trackingNumber, string updateStatus, string logisticName, ref string sErrroCode, ref string sErrorMessage)
        {
            bool bResult = true;
            var dataObject = new
            {
                completeSaleRequest = new
                {
                    requesterCredentials = new
                    {
                        bonanzleAuthToken = AuthToken
                    },
                    shipment = new
                    {
                        shippingTrackingNumber= trackingNumber,
                        shippingCarrierUsed= logisticName
                    },
                    shipped= true,
                    transactionID= orderID

                }
            };

            string dataJSON = JsonConvert.SerializeObject(dataObject);

            var content = new StringContent(dataJSON, Encoding.UTF8, "application/json");


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-bonanzle-api-dev-name", devID);
                client.DefaultRequestHeaders.Add("x-bonanzle-api-cert-name", certID);

                var response = client.PostAsync(ModuleSetings.PostURL, content).Result;
                string responseStr = response.Content.ReadAsStringAsync().Result;

                var responseJSON = JObject.Parse(responseStr);

                // Console.WriteLine(responseJSON);

                var errorMessage = responseJSON["errorMessage"];
                if (errorMessage != null)
                {
                    sErrorMessage = errorMessage.ToString();
                    bResult = false;
                    logger.LogError(errorMessage.ToString());
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
        private bool GetBonanzaDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                marketPlaceID = Convert.ToString(data.Rows[0]["ID"]);
                devID = Convert.ToString(data.Rows[0]["Dev_AccessKey"]);
                certID = Convert.ToString(data.Rows[0]["CertID_AccessKey"]);
                AuthToken = Convert.ToString(data.Rows[0]["Token"]);
            }
            return result;
        }
    }
}