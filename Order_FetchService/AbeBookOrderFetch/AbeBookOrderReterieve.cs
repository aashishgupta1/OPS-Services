using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.XPath;
using DBProject;

namespace AbeBookOrderFetch
{
    public class AbeBookOrderReterieve : ModuleBase
    {
        public string getAllOrderRequest = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><orderUpdateRequest version=\"1.0\"><action name=\"getAllNewOrders\"><username>{0}</username><password>{1}</password></action></orderUpdateRequest>";
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        string userName = string.Empty;
        string password = string.Empty;
        public AbeBookOrderReterieve(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            GetAbeBokDetails();
        }

        public bool GetAbeBokDetails()
        {
            bool result = false;
            DataTable data = GetMarketPlacedetail(ModuleSetings.MarketPlaceName);
            if (data != null && data.Rows.Count > 0)
            {
                userName = Convert.ToString(data.Rows[0]["userName"]);
                password = Convert.ToString(data.Rows[0]["Password"]);
            }
            return result;
        }

        public override void Run()
        {
            GetAllOrder();
        }

        private void GetAllOrder()
        {
            #region Get All Order
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClient httpClient = new HttpClient();
            string requestXML = string.Format(getAllOrderRequest, userName, password);
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

            string strVal = "";
            if (responseMessage.IsSuccessStatusCode)
            {
                strVal = responseMessage.Content.ReadAsStringAsync().Result;
                try
                {
                    orderUpdateResponse orderUpdateResponse = ReadXML(strVal);
                    logger.LogDebug("Response Details :" +  strVal);
                    List<List<string>> orderList = new List<List<string>>();
                    foreach (purchaseOrder purchaseOrder in orderUpdateResponse.purchaseOrderList.purchaseOrder)
                    {
                        string orderID = string.Empty;
                        List<KeyValuePair<string, string>> orderItemList = PrepareQuery(purchaseOrder, ref orderID);
                        SaveintoDB(orderID, orderItemList);
                    }
                }
                catch (Exception e1)
                {
                    logger.LogError(e1.Message);
                }
            }
            #endregion
        }

        public void MapResponse(purchaseOrder purchaseOrder)
        {

            Order_Detail order_Detail = new Order_Detail();
            ORDER_ITEM_DETAIL item_detail = new ORDER_ITEM_DETAIL();
            Order_Other_Detail other_detaiils = new Order_Other_Detail();
            order_Detail.Order_id = purchaseOrder.id;
            order_Detail.PurchaseDate = new DateTime(purchaseOrder.orderDate.date.year, purchaseOrder.orderDate.date.month, purchaseOrder.orderDate.date.day, purchaseOrder.orderDate.time.hour, purchaseOrder.orderDate.time.minute, purchaseOrder.orderDate.time.second, 0);
            order_Detail.OrderStatus = purchaseOrder.status.value;
            order_Detail.SalesChannel = purchaseOrder.domain.name;
            order_Detail.PaymentMethodDetail = purchaseOrder.purchaseMethod;
            order_Detail.BuyerName = purchaseOrder.buyer.mailingAddress.name;
            order_Detail.BuyerName = purchaseOrder.buyer.email;
            order_Detail.gst = Convert.ToDecimal( purchaseOrder.orderTotals.gst.value);
            order_Detail.handling = Convert.ToDecimal(purchaseOrder.orderTotals.handling.value);
            order_Detail.shipping = Convert.ToDecimal(purchaseOrder.orderTotals.shipping.value);
            order_Detail.subtotal = Convert.ToDecimal(purchaseOrder.orderTotals.subtotal.value);
            order_Detail.tax = Convert.ToDecimal(purchaseOrder.orderTotals.tax.value);
            order_Detail.Total_Amount = Convert.ToDecimal(purchaseOrder.orderTotals.total.value);
            order_Detail.CurrencyCode = purchaseOrder.orderTotals.total.currency;
            order_Detail.trackingCode = purchaseOrder.shipping.trackingCode;
            order_Detail.City = purchaseOrder.buyer.mailingAddress.city;
            order_Detail.PostalCode = purchaseOrder.buyer.mailingAddress.code;
            order_Detail.StateOrRegion = purchaseOrder.buyer.mailingAddress.region;
            order_Detail.Phone = purchaseOrder.buyer.mailingAddress.phone;
            order_Detail.Name = purchaseOrder.buyer.mailingAddress.name;
            order_Detail.AddressLine1 = purchaseOrder.buyer.mailingAddress.street;
            order_Detail.AddressLine2 = purchaseOrder.buyer.mailingAddress.street2;
            order_Detail.Country = purchaseOrder.buyer.mailingAddress.country;

            other_detaiils = new Order_Other_Detail();
            other_detaiils.buyer_id = purchaseOrder.buyer.id;
            other_detaiils.buyerPurchaseOrder_id = purchaseOrder.buyerPurchaseOrder.id;
            other_detaiils.reseller_id = purchaseOrder.reseller.id;
            other_detaiils.seller_id = purchaseOrder.seller.id;
            other_detaiils.maxDeliveryDays = purchaseOrder.shipping.maxDeliveryDays;
            other_detaiils.minDeliveryDays = purchaseOrder.shipping.minDeliveryDays;

            foreach (purchaseOrderItem purchaseOrderItem in purchaseOrder.purchaseOrderItemList.purchaseOrderItem)
            {   
                item_detail = new ORDER_ITEM_DETAIL();

                item_detail.OrderItemId = purchaseOrderItem.id;
                item_detail.author = purchaseOrderItem.book.author;
                item_detail.description = purchaseOrderItem.book.description;
            }
        }
        private void SaveintoDB(string orderID, List<KeyValuePair<string, string>> lstQuery)
        {
            foreach (KeyValuePair<string, string> sQuery in lstQuery)
            {
                DataTable dt = new DataTable();
                string orderItemID = sQuery.Key;
                DBOperation.StructDBOperation[] structDBOperations = new DBOperation.StructDBOperation[3];
                int iParam = 0;
                structDBOperations[iParam] = new DBOperation.StructDBOperation();
                structDBOperations[iParam].sParamName = "@orderID";
                structDBOperations[iParam].sParamValue = orderID;
                structDBOperations[iParam].sParamType = System.Data.SqlDbType.VarChar;

                iParam++;
                structDBOperations[iParam] = new DBOperation.StructDBOperation();
                structDBOperations[iParam].sParamName = "@OrderItemID";
                structDBOperations[iParam].sParamValue = orderItemID;
                structDBOperations[iParam].sParamType = System.Data.SqlDbType.NVarChar;


                iParam++;
                structDBOperations[iParam] = new DBOperation.StructDBOperation();
                structDBOperations[iParam].sParamName = "@query";
                structDBOperations[iParam].sParamValue = sQuery.Value;
                structDBOperations[iParam].sParamType = System.Data.SqlDbType.NVarChar;

                string sRetVal = DBOperation.ExecuteDBOperation("sp_InsertIntoOrders", DBOperation.OperationType.STOREDPROC, structDBOperations, ref dt);
                if (sRetVal != "SUCCESS")
                {
                    logger.LogError(sRetVal);
                }
            }

            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    foreach (KeyValuePair<string, string> sQuery in lstQuery)
            //    {
            //        using (SqlCommand command = new SqlCommand("sp_InsertIntoOrders", connection))
            //        {
            //            command.ExecuteNonQuery();
            //        }
            //    }
            //}
        }

        public List<KeyValuePair<string, string>> PrepareQuery(purchaseOrder purchaseOrder, ref string orderID)
        {

            string sOrderQuery = "INSERT INTO OrderDetails(OrderId, buyerID, email, CustomerAddress_city, CustomerAddress_code, CustomerAddress_name, CustomerAddress_country, CustomerAddress_phone, CustomerAddress_region, CustomerAddress_street, CustomerAddress_street2, buyerPurchaseOrderID, domainID, domainName, orderDateTime, orderTotals_gst_amount, orderTotals_gst_currency, orderTotals_handling_amount, orderTotals_handling_currency, orderTotals_shipping_amount, orderTotals_shipping_currency, orderTotals_subtotal_amount, orderTotals_subtotal_currency, orderTotals_tax_amount, orderTotals_tax_currency, orderTotals_total_amount, orderTotals_total_currency, purchaseMethod, OrderItemID, bookID, isbn,author,description,title,Quantity, price, currency, vendorKey, sellerTotal, sellerTotal_currency, status_code, mpstatus, resellerid, sellerid, extraItemShippingCost, extraItemShippingCost_currency, firstItemShippingCost, firstItemShippingCost_currency, maxDeliveryDays, minDeliveryDays, trackingCode, specialInstructions, CurrentProcess, CurrentStatus, Createdon, CreatedBy, LastModifiedOn, LastModifiedBy, IsActive)VALUES("
                    + "'" + purchaseOrder.id + "'"
                    + ",'" + purchaseOrder.buyer.id + "'"
                    + ",'" + purchaseOrder.buyer.email.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.city.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.code + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.name.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.country.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.phone + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.region.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.street.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyer.mailingAddress.street2.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",'" + purchaseOrder.buyerPurchaseOrder.id + "'"
                    + ",'" + purchaseOrder.domain.id + "'"
                    + ",'" + purchaseOrder.domain.name + "'"
                    + ",Convert(datetime, '" + new DateTime(purchaseOrder.orderDate.date.year, purchaseOrder.orderDate.date.month, purchaseOrder.orderDate.date.day, purchaseOrder.orderDate.time.hour, purchaseOrder.orderDate.time.minute, purchaseOrder.orderDate.time.second, 0).ToString("yyyy-MM-dd HH:mm:ss") + "')"
                    + ",'" + purchaseOrder.orderTotals.gst.value + "'"
                    + ",'" + purchaseOrder.orderTotals.gst.currency + "'"
                    + ",'" + purchaseOrder.orderTotals.handling.value + "'"
                    + ",'" + purchaseOrder.orderTotals.handling.currency + "'"
                    + ",'" + purchaseOrder.orderTotals.shipping.value + "'"
                    + ",'" + purchaseOrder.orderTotals.shipping.currency + "'"
                    + ",'" + purchaseOrder.orderTotals.subtotal.value + "'"
                    + ",'" + purchaseOrder.orderTotals.subtotal.currency + "'"
                    + ",'" + purchaseOrder.orderTotals.tax.value + "'"
                    + ",'" + purchaseOrder.orderTotals.tax.currency + "'"
                    + ",'" + purchaseOrder.orderTotals.total.value + "'"
                    + ",'" + purchaseOrder.orderTotals.total.currency + "'"
                    + ",'" + purchaseOrder.purchaseMethod + "'"
                    + "{0}"
                    + ",'" + purchaseOrder.reseller.id + "'"
                    + ",'" + purchaseOrder.seller.id + "'"
                    + ",'" + purchaseOrder.shipping.extraItemShippingCost.value + "'"
                    + ",'" + purchaseOrder.shipping.extraItemShippingCost.currency + "'"
                    + ",'" + purchaseOrder.shipping.firstItemShippingCost.value + "'"
                    + ",'" + purchaseOrder.shipping.firstItemShippingCost.currency + "'"
                    + ",'" + purchaseOrder.shipping.maxDeliveryDays + "'"
                    + ",'" + purchaseOrder.shipping.minDeliveryDays + "'"
                    + ",'" + purchaseOrder.shipping.trackingCode + "'"
                    + ",'" + purchaseOrder.specialInstructions.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'"
                    + ",1"
                    + ",'InProcess'"
                    + ", getdate()"
                    + ",2"
                    + ",NULL,NULL, 1)";

            List<KeyValuePair<string, string>> lstOrderItemQuery = new List<KeyValuePair<string, string>>();
            foreach (purchaseOrderItem orderItem in purchaseOrder.purchaseOrderItemList.purchaseOrderItem)
            {
                string sOrderItem = string.Empty;
                sOrderItem += ",'" + orderItem.id + "'";
                sOrderItem += ",'" + orderItem.book.id + "'";
                sOrderItem += ",'" + orderItem.book.isbn + "'";
                sOrderItem += ",'" + orderItem.book.author.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'";
                sOrderItem += ",'" + (orderItem.book.description.Length > 1500 ? orderItem.book.description.Substring(0, 1500) : orderItem.book.description).Replace("'", "").Replace("’", "").Replace("‘", "") + "'";
                sOrderItem += ",'" + orderItem.book.title.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'";
                sOrderItem += ",'1'";
                sOrderItem += ",'" + orderItem.book.price.value + "'";
                sOrderItem += ",'" + orderItem.book.price.currency + "'";
                sOrderItem += ",'" + orderItem.book.vendorKey.Replace("'", "''").Replace("’", "").Replace("‘", "") + "'";
                sOrderItem += ",'" + orderItem.sellerTotal.value + "'";
                sOrderItem += ",'" + orderItem.sellerTotal.currency + "'";
                sOrderItem += ",'" + orderItem.status.code + "'";
                sOrderItem += ",'" + orderItem.status.value.Replace("'", "''").Replace("’", "") + "'";
                string sQuery = string.Format(sOrderQuery, sOrderItem.Replace("--", "").Replace("‘", "").Replace("’", ""));
                lstOrderItemQuery.Add(new KeyValuePair<string, string>(orderItem.id, sQuery));
            }
            orderID = purchaseOrder.id;
            return lstOrderItemQuery;
        }

        public orderUpdateResponse ReadXML(string strData)
        {
            orderUpdateResponse result = new orderUpdateResponse();

            XPathDocument xmlDoc = new XPathDocument(new StringReader(strData));
            XPathNavigator navigator = xmlDoc.CreateNavigator();
            result.purchaseOrderList = new purchaseOrderList();
            result.purchaseOrderList.purchaseOrder = new List<purchaseOrder>();
            XPathNodeIterator iterator = navigator.Select("orderUpdateResponse/purchaseOrderList/purchaseOrder");
            while (iterator.MoveNext())
            {
                purchaseOrder purchaseOrder = new purchaseOrder();

                purchaseOrder.id = iterator.Current.GetAttribute("id", "");
                #region buyer
                purchaseOrder.buyer = new buyer();
                purchaseOrder.buyer.id = iterator.Current.Select("buyer").Current.GetAttribute("id", "");
                purchaseOrder.buyer.email = GetConfigValue(iterator.Current.SelectSingleNode("buyer"), "email");
                purchaseOrder.buyer.mailingAddress = new mailingAddress();
                purchaseOrder.buyer.mailingAddress = GetMailingAddress(iterator.Current.SelectSingleNode("buyer/mailingAddress"));
                #endregion

                #region buyerPurchaseOrder
                purchaseOrder.buyerPurchaseOrder = new buyerPurchaseOrder();
                purchaseOrder.buyerPurchaseOrder.id = iterator.Current.Select("buyerPurchaseOrder").Current.GetAttribute("id", "");
                #endregion

                #region domain
                purchaseOrder.domain = new domain();
                purchaseOrder.domain.id = iterator.Current.Select("domain").Current.GetAttribute("id", "");
                purchaseOrder.domain.name = GetConfigValue(iterator.Current.SelectSingleNode("domain"), "name");
                #endregion

                #region orderDate
                purchaseOrder.orderDate = new orderDate();
                purchaseOrder.orderDate = GetOrderDateDetail(iterator.Current.SelectSingleNode("orderDate"));
                #endregion

                #region orderTotals
                purchaseOrder.orderTotals = new orderTotals();
                purchaseOrder.orderTotals = GetOrderTotals(iterator.Current.SelectSingleNode("orderTotals"));
                #endregion

                #region purchaseMethod
                purchaseOrder.purchaseMethod = GetConfigValue(iterator.Current, "purchaseMethod");
                #endregion

                #region purchaseOrderItemList
                purchaseOrder.purchaseOrderItemList = new purchaseOrderItemList();
                purchaseOrder.purchaseOrderItemList = GetPurchaseOrderItemList(iterator.Current.SelectSingleNode("purchaseOrderItemList"));
                #endregion

                #region reseller
                purchaseOrder.reseller = new reseller();
                purchaseOrder.reseller.id = iterator.Current.Select("reseller").Current.GetAttribute("id", "");
                purchaseOrder.reseller.name = GetConfigValue(iterator.Current.SelectSingleNode("reseller"), "name");
                #endregion

                #region seller
                purchaseOrder.seller = new seller();
                purchaseOrder.seller.id = iterator.Current.Select("seller").Current.GetAttribute("id", "");
                #endregion

                #region shipping
                purchaseOrder.shipping = new purchaseOrdershipping();
                purchaseOrder.shipping = getShippingDetails(iterator.Current.SelectSingleNode("shipping"));
                #endregion

                #region status
                purchaseOrder.status = new status();
                purchaseOrder.status.code = iterator.Current.SelectSingleNode("status").GetAttribute("code", "");
                purchaseOrder.status.value = GetConfigValue(iterator.Current, "status");
                #endregion

                #region specialInstructions
                purchaseOrder.specialInstructions = GetConfigValue(iterator.Current, "specialInstructions");
                #endregion
                result.purchaseOrderList.purchaseOrder.Add(purchaseOrder);

            }
            return result;

        }

        public purchaseOrderItemList GetPurchaseOrderItemList(XPathNavigator navigator)
        {
            purchaseOrderItemList result = new purchaseOrderItemList();
            result.purchaseOrderItem = new List<purchaseOrderItem>();
            XPathNodeIterator nodeIterator = navigator.Select("purchaseOrderItem");
            while (nodeIterator.MoveNext())
            {
                purchaseOrderItem item = new purchaseOrderItem();
                item = GetPurchaseOrderItem(nodeIterator.Current);
                result.purchaseOrderItem.Add(item);
            }
            return result;
        }

        public purchaseOrderItem GetPurchaseOrderItem(XPathNavigator navigator)
        {
            purchaseOrderItem result = new purchaseOrderItem();
            result.id = navigator.GetAttribute("id", "");
            result.book = GetBookDetails(navigator.SelectSingleNode("book"));
            result.orderDate = GetOrderDateDetail(navigator.SelectSingleNode("orderDate"));
            result.sellerTotal = new sellerTotal();
            result.sellerTotal.currency = navigator.SelectSingleNode("sellerTotal").GetAttribute("currency", "");
            result.sellerTotal.value = GetConfigValue(navigator, "sellerTotal");
            result.status = new status();
            result.status.code = navigator.SelectSingleNode("status").GetAttribute("code", "");
            result.status.value = GetConfigValue(navigator, "status");

            return result;
        }
        public book GetBookDetails(XPathNavigator navigator)
        {
            book result = new book();
            result.id = navigator.GetAttribute("id", "");
            result.isbn = GetConfigValue(navigator, "isbn");
            result.author = GetConfigValue(navigator, "author");
            result.description = GetConfigValue(navigator, "description");
            result.title = GetConfigValue(navigator, "title");
            result.vendorKey = GetConfigValue(navigator, "vendorKey");
            result.price = new price();
            result.price.currency = navigator.SelectSingleNode("price").GetAttribute("currency", "");
            result.price.value = GetConfigValue(navigator, "price");
            return result;
        }
        public purchaseOrdershipping getShippingDetails(XPathNavigator navigator)
        {
            purchaseOrdershipping result = new purchaseOrdershipping();
            result.company = GetConfigValue(navigator, "company");
            result.minDeliveryDays = GetConfigValue(navigator, "minDeliveryDays");
            result.maxDeliveryDays = GetConfigValue(navigator, "maxDeliveryDays");
            result.trackingCode = GetConfigValue(navigator, "trackingCode");
            result.extraItemShippingCost = new extraItemShippingCost();
            result.extraItemShippingCost.currency = navigator.SelectSingleNode("extraItemShippingCost").GetAttribute("currency", "");
            result.extraItemShippingCost.value = GetConfigValue(navigator, "extraItemShippingCost");

            result.firstItemShippingCost = new firstItemShippingCost();
            result.firstItemShippingCost.currency = navigator.SelectSingleNode("firstItemShippingCost").GetAttribute("currency", "");
            result.firstItemShippingCost.value = GetConfigValue(navigator, "firstItemShippingCost");
            return result;
        }
        public orderTotals GetOrderTotals(XPathNavigator navigator)
        {
            orderTotals result = new orderTotals();
            result.gst = new gst();
            result.gst.currency = navigator.SelectSingleNode("gst").GetAttribute("currency", "");
            result.gst.value = GetConfigValue(navigator, "gst");

            result.handling = new handling();
            result.handling.currency = navigator.SelectSingleNode("handling").GetAttribute("currency", "");
            result.handling.value = GetConfigValue(navigator, "handling");


            result.shipping = new shipping();
            result.shipping.currency = navigator.SelectSingleNode("shipping").GetAttribute("currency", "");
            result.shipping.value = GetConfigValue(navigator, "shipping");


            result.subtotal = new subtotal();
            result.subtotal.currency = navigator.SelectSingleNode("subtotal").GetAttribute("currency", "");
            result.subtotal.value = GetConfigValue(navigator, "subtotal");

            result.tax = new tax();
            result.tax.currency = navigator.SelectSingleNode("tax").GetAttribute("currency", "");
            result.tax.value = GetConfigValue(navigator, "tax");

            result.total = new total();
            result.total.currency = navigator.SelectSingleNode("total").GetAttribute("currency", "");
            result.total.value = GetConfigValue(navigator, "total");
            return result;
        }

        public orderDate GetOrderDateDetail(XPathNavigator navigator)
        {
            orderDate result = new orderDate
            {
                date = new date(),
                time = new time()
            };
            result.date.day = Convert.ToInt32(GetConfigValue(navigator.SelectSingleNode("date"), "day"));
            result.date.month = Convert.ToInt32(GetConfigValue(navigator.SelectSingleNode("date"), "month"));
            result.date.year = Convert.ToInt32(GetConfigValue(navigator.SelectSingleNode("date"), "year"));
            result.time.hour = Convert.ToInt32(GetConfigValue(navigator.SelectSingleNode("time"), "hour"));
            result.time.minute = Convert.ToInt32(GetConfigValue(navigator.SelectSingleNode("time"), "minute"));
            result.time.second = Convert.ToInt32(GetConfigValue(navigator.SelectSingleNode("time"), "second"));

            return result;
        }

        public mailingAddress GetMailingAddress(XPathNavigator navigator)
        {
            mailingAddress address = new mailingAddress();
            address.city = GetConfigValue(navigator, "city");
            address.code = GetConfigValue(navigator, "code");
            address.country = GetConfigValue(navigator, "country");
            address.name = GetConfigValue(navigator, "name");
            address.phone = GetConfigValue(navigator, "phone");
            address.region = GetConfigValue(navigator, "region");
            address.street = GetConfigValue(navigator, "street");
            address.street2 = GetConfigValue(navigator, "street2");
            return address;
        }

        internal static string GetConfigValue(XPathNavigator navigator, string xpath, bool trim = true)
        {
            XPathNavigator valNav = navigator.SelectSingleNode(xpath);
            if (valNav != null)
            {
                return trim ? valNav.Value.Trim() : valNav.Value;
            }
            return string.Empty;
        }
    }
}