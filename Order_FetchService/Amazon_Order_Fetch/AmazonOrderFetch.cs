using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using ModuleCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Amazon_Order_Fetch
{
    public class AmazonOrderFetch : ModuleBase
    {
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        string sellerId = "A2VGN7ZF4RSVOT";

        string mwsAuthToken = "amzn.mws.e6b42836-2b6b-212b-b6e8-9c50ba52a33c";

        // Developer AWS access key
        string accessKey = "AKIAJVEHY7BN2C4SYK3Q";

        // Developer AWS secret key
        string secretKey = "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";

        // The client application name
        string appName = "AmzonMWSOrderService";

        // The client application version
        string appVersion = "1.0";

        string serviceURL = "https://mws.amazonservices.com";

        List<string> marketplaceId = new List<string> { "A1AM78C64UM0Y8", "A2EUQ1WTGCTBG2", "ATVPDKIKX0DER" };

        private readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders client;
        public AmazonOrderFetch(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            GetAmazonUSDetails();
        }
        private bool GetAmazonUSDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                sellerId = Convert.ToString(data.Rows[0]["sellerId"]);
                mwsAuthToken = Convert.ToString(data.Rows[0]["Token"]);
                accessKey = Convert.ToString(data.Rows[0]["Dev_AccessKey"]);
                secretKey = Convert.ToString(data.Rows[0]["CertID_SecretKey"]);
                string lstmarketplaceId = Convert.ToString(data.Rows[0]["MarketPlaceID"]);
                marketplaceId= lstmarketplaceId.Split(',').ToList();

            }
            return result;
        }

        public override void Run()
        {
            OrderFetch();
        }

        private void OrderFetch()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string assemblyFile = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            xmlDoc.Load(Path.Combine(assemblyFile, "LastFetchedInfo.xml"));
            XmlNodeList lstnode = xmlDoc.SelectNodes("/LastFecthedTime");
            XmlNode node = null;
            if (lstnode != null)
            {
                node = lstnode.Item(lstnode.Count - 1);
            }
            
            DateTime lastFecthDateTime = DateTime.Now;

            if (node != null && !string.IsNullOrEmpty(node.InnerText))
            {
                lastFecthDateTime = Convert.ToDateTime(node.InnerText);
            }
            var v = GetOrder(lastFecthDateTime.Subtract(new TimeSpan(1,0,0,0 )));
            if(v != null)
            {
                ListOrdersResult result = ((MarketplaceWebServiceOrders.Model.ListOrdersResponse)v).ListOrdersResult;
                foreach(Order order in  result.Orders)
                {
                     MapOrderToDBModel(order);
                    SaveToDB(order);
                }
            }
        }

        public void MapOrderToDBModel(Order order)
        {

        }
        private bool SaveToDB(Order order)
        {
            bool bResult = false;

            return bResult;
        }

        private IMWSResponse GetOrder(DateTime createdAfter)
        {
            var config = new MarketplaceWebServiceOrdersConfig { ServiceURL = ModuleSetings.PostURL };
            var client = new MarketplaceWebServiceOrdersClient(accessKey, secretKey, appName, appVersion, config);

            try
            {
                ListOrdersRequest request = new ListOrdersRequest();

                request.SellerId = sellerId;
                request.MWSAuthToken = mwsAuthToken;
                request.CreatedAfter = createdAfter;
                request.MarketplaceId = marketplaceId;

                return client.ListOrders(request);
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                //// Exception properties are important for diagnostics.
                //ResponseHeaderMetadata rhmd = ex.ResponseHeaderMetadata;
                //Console.WriteLine("Service Exception:");
                //if (rhmd != null)
                //{
                //    Console.WriteLine("RequestId: " + rhmd.RequestId);
                //    Console.WriteLine("Timestamp: " + rhmd.Timestamp);
                //}
                //Console.WriteLine("Message: " + ex.Message);
                //Console.WriteLine("StatusCode: " + ex.StatusCode);
                //Console.WriteLine("ErrorCode: " + ex.ErrorCode);
                //Console.WriteLine("ErrorType: " + ex.ErrorType);
                throw ex;
            }
        }
    }
}
