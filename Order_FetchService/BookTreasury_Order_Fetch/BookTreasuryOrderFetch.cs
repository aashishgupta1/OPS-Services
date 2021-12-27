using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using ModuleCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DBProject;
using DBInteraction;
using ModuleCore.Module.OrderHelper;
using System.Globalization;

namespace BookTreasury_Order_Fetch
{
    public class BookTreasuryOrderFetch : ModuleBase
    {
        bool autoFetch = false;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public BookTreasuryOrderFetch(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
        }

        public override void Run()
        {
            try
            {
                if (GetBookTreasuryDetails())
                {
                    if (autoFetch)
                        if(OrderFetch())
                        {
                            logger.LogError(ModuleSetings.MarketPlaceName + "Error in order fetching");
                        }
                }
            }
            catch (Exception e)
            {
                logger.LogError("Mesage :" + e.Message + "\n Stack trace : " + e.StackTrace);
            }
        }
        private bool GetBookTreasuryDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {  
                autoFetch = Convert.ToBoolean(data.Rows[0]["AutoFetch"]);
                result = true;
            }
            return result;
        }

        private bool OrderFetch()
        {
            bool result = false;
            string url = ModuleSetings.PostURL;
            HttpResponseMessage HttpResponseMessage = null;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage = httpClient.GetAsync(url).Result;
                if (HttpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string sData = "{ \"data\" :" + HttpResponseMessage.Content.ReadAsStringAsync().Result.Trim() + "}";
                    OrderResponse obj = JsonConvert.DeserializeObject<OrderResponse>(sData);
                    InsertToOrder(obj);
                    result = true;
                }
                else
                {
                    logger.LogError("Issue in Getting Order from BookTreasury : " + HttpResponseMessage.StatusCode + ":" + HttpResponseMessage.Content.ToString());
                }
            }
            return result;
        }
        private void InsertToOrder(OrderResponse order)
        {
            List<Order_Detail> lstorder_Detail = new List<Order_Detail>();
            Order_Detail objorder_Detail = new Order_Detail();
            Order_Other_Detail objorder_Other_Detail = new Order_Other_Detail();
            foreach (Datum datum in order.data)
            {
                if (OrderHelper.GetOrderDetailsByOrderID(datum.order_no).Count() == 0)
                {
                    int iTotalCount = 0;
                    objorder_Detail = new Order_Detail
                    {
                        Order_id = datum.order_no,
                        PurchaseDate = DateTime.ParseExact(datum.order_date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        OrderStatus = datum.status,
                        BuyerName = datum.customer.full_name,
                        shipping = datum.shipping_charges,
                        Total_Amount = (decimal)datum.total,
                        CurrencyCode = datum.currency,
                        City = datum.customer.shipping_town_city,
                        PostalCode = datum.customer.shipping_postal_zip_code,
                        StateOrRegion = datum.customer.shipping_state,
                        Phone = Convert.ToString(datum.customer.shipping_phone),
                        Name = datum.customer.shipping_full_name,
                        AddressLine1 = datum.customer.shipping_address,
                        Country = datum.customer.shipping_country
                    };

                    objorder_Other_Detail = new Order_Other_Detail()
                    {
                        orderremarks = datum.order_date,
                        couponcode = datum.coupon_id,
                        coupondisc = (decimal)datum.coupon_amount
                        //gift = datum.gift,
                        //box = datum.box,
                        //shipping_type = datum.shipping_type,
                        //referral_id = datum.referral_id,
                        //referral_amount = datum.referral_amount
                    };
                    foreach(Order_Items order_Items in  datum.order_items )
                    {
                        ORDER_ITEM_DETAIL item_detail = new ORDER_ITEM_DETAIL();
                        item_detail.OrderItemId = order_Items.book_id;
                        item_detail.Quantity = order_Items.quantity;
                        item_detail.ItemPrice = (decimal)order_Items.price;
                        iTotalCount += order_Items.quantity;
                    }

                    objorder_Detail.Order_Quantity = iTotalCount;

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

        }
    }
}
