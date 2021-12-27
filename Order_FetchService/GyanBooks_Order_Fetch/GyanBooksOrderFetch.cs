using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DBProject;
using Newtonsoft.Json;
using ModuleCore.Module.OrderHelper;

namespace GyanBooks_Order_Fetch
{
    class GyanBooksOrderFetch : ModuleBase
    {
        string userName = string.Empty;
        string password = string.Empty;
        bool autoFetch = false;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";

        public GyanBooksOrderFetch(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            
        }

        private bool GetGyanBooksDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                userName = Convert.ToString(data.Rows[0]["userName"]);
                password = Convert.ToString(data.Rows[0]["Password"]);
                autoFetch = Convert.ToBoolean(data.Rows[0]["AutoFetch"]);
            }
            return result;
        }
        public override void Run()
        {
            try
            {
                GetGyanBooksDetails();
                if (autoFetch)
                    if(!OrderFetch())
                    {
                        logger.LogError(ModuleSetings.MarketPlaceName + "Error in order fetching");
                    }
            }
            catch (Exception e)
            {
                logger.LogError("Mesage :" + e.Message + "\n Stack trace : " + e.StackTrace);
            }
        }
        private bool OrderFetch()
        {
            bool result = false;
            string url = ModuleSetings.PostURL;
            HttpResponseMessage HttpResponseMessage = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ModuleSetings.PostURL);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.ASCIIEncoding.ASCII.GetBytes(
                   $"{userName}:{password}")));
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage = httpClient.GetAsync(url).Result;
                if (HttpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var response =  "{ \"data\" :" + HttpResponseMessage.Content.ReadAsStringAsync().Result.Trim() + "}";
                    OrderAPIResponse order = JsonConvert.DeserializeObject<OrderAPIResponse>(response);
                    InsertToOrder(order);
                    result = true;
                }
                else
                {
                    logger.LogError("Issue in Getting Order from BookTreasury : " + HttpResponseMessage.StatusCode + ":" + HttpResponseMessage.Content.ToString());
                }
            }
            return result;
        }
        private void InsertToOrder(OrderAPIResponse order)
        {
            List<Order_Detail> lstorder_Detail = new List<Order_Detail>();
            Order_Detail objorder_Detail = new Order_Detail();
            Order_Other_Detail objorder_Other_Detail = new Order_Other_Detail();
            foreach (Datum datum in order.data)
            {
                if (OrderHelper.GetOrderDetailsByOrderID(datum.orderid).Count() == 0)
                {
                    int iIndex = lstorder_Detail.FindIndex(x => x.Order_id == datum.orderid);
                    if (iIndex != -1)
                    {
                        ORDER_ITEM_DETAIL objorder_ITEM_DETAIL = new ORDER_ITEM_DETAIL();
                        objorder_ITEM_DETAIL.TotalPrice = Convert.ToDecimal(datum.amount);
                        objorder_ITEM_DETAIL.ItemPrice = Convert.ToDecimal(datum.rate);
                        objorder_ITEM_DETAIL.BINDING = datum.binding;
                        objorder_ITEM_DETAIL.OrderItemId = datum.bookcode;
                        objorder_ITEM_DETAIL.description = datum.bookdesc;
                        objorder_ITEM_DETAIL.CurrencyCode = datum.currency;
                        objorder_ITEM_DETAIL.Quantity = Convert.ToInt32(datum.qty);
                        lstorder_Detail[iIndex].ORDER_ITEM_DETAIL.Add(objorder_ITEM_DETAIL);
                        lstorder_Detail[iIndex].Order_Quantity += Convert.ToInt32(datum.qty);
                    }
                    else
                    {
                        #region Order details
                        objorder_Detail = new Order_Detail
                        {
                            Order_id = datum.orderid,
                            PurchaseDate = Convert.ToDateTime(datum.orderdate),
                            OrderStatus = datum.orderstatus,
                            payment_Status = datum.paymentstatus,
                            BuyerName = datum.ordname,
                            BuyerEmail = datum.ordemailid,
                            Total_Amount = Convert.ToDecimal(datum.ordamt),
                            CurrencyCode = datum.ordcurr,
                            City = datum.ordcity,
                            PostalCode = datum.ordzip,
                            StateOrRegion = datum.ordstate,
                            Phone = datum.ordphone,
                            AddressLine1 = datum.ordaddress,
                            Country = datum.ordcountry,
                            ordmobile = datum.ordmobile
                        };

                        objorder_Other_Detail = new Order_Other_Detail
                        {
                            couponcode = datum.couponcode,
                            coupondisc = string.IsNullOrEmpty(datum.coupondisc) ? 0 : Convert.ToDecimal(datum.coupondisc),
                            ipaddress = datum.ipaddress,
                            lastupdate = datum.lastupdate,
                            orddiscount = Convert.ToDecimal(datum.orddiscount),
                            ordorganisation = datum.ordorganisation,
                            orddepartment = datum.orddepartment,
                            remarks = datum.remarks,
                            paymentremarks = datum.paymentremarks,
                            orderremarks = datum.orderremarks,
                            shippingstatus = datum.shippingstatus,
                            shippingremarks = datum.shippingremarks,
                            toshow = datum.toshow,
                            ordlogs = datum.ordlogs,
                            PaymentMethod = datum.paymentmode,
                            orderid = datum.orderid,
                            orderid_ordersdet = datum.orderid_ordersdet,
                            lastupdate_ordersdet = datum.lastupdate_ordersdet
                        };


                        objorder_Detail.Order_Other_Detail.Add(objorder_Other_Detail);
                        #endregion

                        #region Order Item Details
                        ORDER_ITEM_DETAIL objorder_ITEM_DETAIL = new ORDER_ITEM_DETAIL();
                        objorder_ITEM_DETAIL.TotalPrice = Convert.ToDecimal(datum.amount);
                        objorder_ITEM_DETAIL.ItemPrice = Convert.ToDecimal(datum.rate);
                        objorder_ITEM_DETAIL.BINDING = datum.binding;
                        objorder_ITEM_DETAIL.OrderItemId = datum.bookcode;
                        objorder_ITEM_DETAIL.description = datum.bookdesc;
                        objorder_ITEM_DETAIL.CurrencyCode = datum.currency;
                        objorder_ITEM_DETAIL.Quantity = Convert.ToInt32(datum.qty);

                        objorder_Detail.ORDER_ITEM_DETAIL.Add(objorder_ITEM_DETAIL);

                        objorder_Detail.Order_Quantity = Convert.ToInt32(datum.qty);
                        #endregion

                        lstorder_Detail.Add(objorder_Detail);
                    }
                }
            }
            foreach (Order_Detail detail in lstorder_Detail)
            {
                try
                {
                    OrderHelper.InsertOrderInfo(detail);
                }
                catch (Exception e)
                {
                    logger.LogError($"Exception in inserting Record: {objorder_Detail.Order_id} : {e.Message}");
                }
            }
        }
    }
}
