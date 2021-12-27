using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using Bonanza_Order_Fetch.Model;
using DBInteraction;
using ModuleCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;
using DBProject;
using ModuleCore.Module.OrderHelper;

namespace Bonanza_Order_Fetch
{
    public class BonanzaOrderFetch : ModuleBase
    {
        public string devID = string.Empty;
        public string certID = string.Empty;
        public string AuthToken = string.Empty;
        public bool AutoFetch = false;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public BonanzaOrderFetch(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            
        }

        public override void Run()
        {
            try
            {
                if (GetBonanzaDetails())
                {
                    if (AutoFetch)
                        OrderFetch();
                }
            }
            catch(Exception e)
            {
                logger.LogError("Mesage :" + e.Message + "\n Stack trace : " + e.StackTrace);
            }
        }
        private void OrderFetch()
        {  
            GetOrderAndProcess();
        }

        private DateTime getLastProcesedDateTime()
        {
            DateTime lastFecthDateTime = DateTime.Now;


            XmlDocument xmlDoc = new XmlDocument();
            string assemblyFile = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            xmlDoc.Load(Path.Combine(assemblyFile, "LastFetchedInfo.xml"));
            XmlNodeList lstnode = xmlDoc.SelectNodes("/LastFecthedTime");
            XmlNode node = null;
            if (lstnode != null)
            {
                node = lstnode.Item(lstnode.Count - 1);
            }

            

            if (node != null && !string.IsNullOrEmpty(node.InnerText))
            {
                lastFecthDateTime = Convert.ToDateTime(node.InnerText);
            }
            return lastFecthDateTime;
        }
        private void GetOrderAndProcess()
        {

            DateTime lastFecthDateTime = getLastProcesedDateTime();
            List<OrderResponse> lstOrderResponses = new List<OrderResponse>();
            OrderResponse order = GetOrder(1, lastFecthDateTime);
            lstOrderResponses.Add(order);
            while (order.getOrdersResponse.pageNumber != order.getOrdersResponse.paginationResult.totalNumberOfPages)
            {
                order=  GetOrder(order.getOrdersResponse.pageNumber + 1, lastFecthDateTime);
                lstOrderResponses.Add(order);
            }

            ProcessOrder(lstOrderResponses, lastFecthDateTime);
        }

        private OrderResponse GetOrder(int iPageNumber, DateTime lastFecthDateTime)
        {
            OrderResponse orderDetails = null;
            
            
            var dataObject = new
            {
                getOrdersRequest = new
                {
                    requesterCredentials = new
                    {
                        bonanzleAuthToken = AuthToken
                    },
                    orderRole = "Seller",
                    createTimeFrom = DateTime.Now.Subtract(new TimeSpan(150,0,0,0,0)), //lastFecthDateTime,
                    createTimeTo = DateTime.Now,
                    paginationInput = new
                    {
                        entriesPerPage = 5,
                        pageNumber =iPageNumber
                    }
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

                JObject responseJSON = JObject.Parse(responseStr);

                var errorMessage = responseJSON["errorMessage"];
                if (errorMessage != null)
                {
                    logger.LogError(errorMessage.ToString());
                }
                else
                {
                    orderDetails = JsonConvert.DeserializeObject<OrderResponse>(responseStr);
                    
                }
            }
            return orderDetails;
        }

        private void ProcessOrder(List<OrderResponse> orderDetails, DateTime lastFecthDateTime)
        {
            foreach( OrderResponse response in orderDetails)
            {
                SaveIntoDB(response);
            }

            XmlDocument xmlDoc = new XmlDocument();
            string assemblyFile = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            xmlDoc.Load(Path.Combine(assemblyFile, "LastFetchedInfo.xml"));

            XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LastFecthedTime", "");
            xmlNode.InnerText = lastFecthDateTime.ToString("dd-MM-yyyy HH:mm:sss");
            xmlDoc.DocumentElement.AppendChild(xmlNode);
            xmlDoc.Save(Path.Combine(assemblyFile, "LastFetchedInfo.xml"));

        }

        public bool SaveIntoDB(OrderResponse orderDetails)
        {
            bool bResult = false;
            try
            {
                Order_Detail objorder_Detail = new Order_Detail();
                Order_Other_Detail objorder_Other_Detail = new Order_Other_Detail();

                foreach (Orderarray orderarray in  orderDetails.getOrdersResponse.orderArray)
                {
                    if (OrderHelper.GetOrderDetailsByOrderID(Convert.ToString(orderarray.order.orderID)).Count() == 0)
                    {
                        objorder_Detail = new Order_Detail
                        {
                            Order_id = Convert.ToString(orderarray.order.orderID),
                            PurchaseDate = orderarray.order.createdTime,
                            OrderStatus = orderarray.order.orderStatus,
                            Payment_Date = orderarray.order.paidTime,
                            BuyerName = orderarray.order.shippingAddress.name,
                            BuyerEmail = orderarray.order.transactionArray.transaction.buyer.email,

                            CurrencyCode = orderarray.order.currencyCode,
                            trackingCode = orderarray.order.shippingDetails.shipmentTrackingNumber,
                            City = orderarray.order.shippingAddress.cityName,
                            PostalCode = orderarray.order.shippingAddress.postalCode,
                            StateOrRegion = orderarray.order.shippingAddress.stateOrProvince,
                            CountryCode = orderarray.order.shippingAddress.country,
                            Name = orderarray.order.shippingAddress.name,
                            AddressLine1 = orderarray.order.shippingAddress.street1,
                            AddressLine2 = orderarray.order.shippingAddress.street2,
                            Country = orderarray.order.shippingAddress.countryName,
                            Ship_Method = orderarray.order.shippingDetails.shippingService,
                            subtotal = (decimal)orderarray.order.subtotal,
                            tax = (decimal)orderarray.order.taxAmount
                        };

                        decimal.TryParse(orderarray.order.shippingDetails.amount, out decimal amount);
                        objorder_Detail.shipping = amount;
                        amount = 0;
                        decimal.TryParse(orderarray.order.total, out amount);
                        objorder_Detail.Total_Amount = amount;

                        objorder_Other_Detail = new Order_Other_Detail
                        {
                            shippingremarks = orderarray.order.shippingDetails.notes,
                            orderremarks = orderarray.order.buyerCheckoutMessage,
                            PaymentMethod = orderarray.order.transactionArray.transaction.providerName,
                            payments_transaction_id = orderarray.order.transactionArray.transaction.providerID
                            //finalValueFee = orderarray.order.transactionArray.transaction.finalValueFee,
                            //buyerUserID = orderarray.order.buyerUserID,
                            //buyerUserName = orderarray.order.buyerUserName,
                            //creatingUserRole = orderarray.order.creatingUserRole,
                            //insuranceFee = orderarray.order.shippingDetails.insuranceFee
                        };

                        objorder_Detail.Order_Other_Detail.Add(objorder_Other_Detail);

                        foreach (Itemarray itemarray in orderarray.order.itemArray)
                        {
                            ORDER_ITEM_DETAIL item_detail = new ORDER_ITEM_DETAIL
                            {
                                OrderItemId = Convert.ToString(itemarray.item.itemID),
                                ItemCode = itemarray.item.sku,
                                Title = itemarray.item.title,
                                Quantity = itemarray.item.quantity,
                                ItemPrice = Convert.ToInt32(itemarray.item.price),
                                //personalizedText = itemarray.item.personalizedText
                            };

                            objorder_Detail.Order_Quantity += item_detail.Quantity;
                            objorder_Detail.ORDER_ITEM_DETAIL.Add(item_detail);
                        }

                        try
                        {
                            OrderHelper.InsertOrderInfo(objorder_Detail);
                        }
                        catch (Exception e)
                        {
                            logger.LogError($"Exception in inserting Record: {objorder_Detail.Order_id} : {e.Message}");
                        }
                    }
                }
                bResult = true;
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
            }
            return bResult;
        }


        public bool GetBonanzaDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                devID = Convert.ToString(data.Rows[0]["Dev_AccessKey"]);
                certID = Convert.ToString(data.Rows[0]["Certid_Secretkey"]);
                AuthToken = Convert.ToString(data.Rows[0]["Token"]);
                AutoFetch = Convert.ToBoolean(data.Rows[0]["AutoFetch"]);
                result = true;
            }
            return result;

        }
        private void tokenFetch()
        {

            var dataObject = new { fetchTokenRequest = new { } };

            string dataJSON = JsonConvert.SerializeObject(dataObject);

            var url = ModuleSetings.PostURL;
            var content = new StringContent(dataJSON, Encoding.UTF8, "application/json");
            

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-bonanzle-api-dev-name", devID);
                client.DefaultRequestHeaders.Add("x-bonanzle-api-cert-name", certID);

                var response = client.PostAsync(url, content).Result;
                string responseStr = response.Content.ReadAsStringAsync().Result;

                var responseJSON = JObject.Parse(responseStr);

                //Console.WriteLine(responseJSON);
                string result = responseJSON.ToString();
                Rootobject rootobject=  JsonConvert.DeserializeObject<Rootobject>(result);
                AuthToken = rootobject.fetchTokenResponse.authToken;
                var errorMessage = responseJSON["errorMessage"];
                if (errorMessage != null)
                {
                    //Console.WriteLine(errorMessage);
                }
            }

        }

    }
}
