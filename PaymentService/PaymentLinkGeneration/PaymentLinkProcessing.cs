using OPSService.Infrastructure;
using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using Newtonsoft.Json;
using PaymentLinkOrderFilteration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PaymentLinkGeneration
{
    public class PaymentLinkProcessing : ModuleBase
    {
        #region Constatnt
        public const string ORDERID_PLACEHOLDER = "<<OrderID>>";
        public const string CLIENTNAME_PLACEHOLDER = "<<ClientName>>";
        public const string PAYMENTLINK_PLACEHOLDER = "<<PaymentLink>>";
        public const string ORDERITEMLIST_PLACEHOLDER = "<<OrderItemList>>";

        #endregion
        private const decimal V = 0.0001M;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public PaymentLinkProcessing(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
        }

        public override void Run()
        {
            DataTable table = GetRecordForLinkShare();

            List<string> val = table.AsEnumerable().Select(x => x.Field<string>("PaymentGatewayName")).Distinct().ToList();
            string paymentGatewayName = string.Join(",", val);
            List<PaymentGatewayDetails> paymentGatewayDetails = GetPaymentGatewayDetails(paymentGatewayName);
            GenerateLink(table, paymentGatewayDetails);
        }

        public List<PaymentGatewayDetails> GetPaymentGatewayDetails(string paymentGatewayName)
        {
            List<PaymentGatewayDetails> gatewayDetails = new List<PaymentGatewayDetails>();
            DBOperation.StructDBOperation[] dBOperations = new DBOperation.StructDBOperation[1];
            dBOperations[0] = new DBOperation.StructDBOperation();
            dBOperations[0].sParamName = "@paymentGateways";
            dBOperations[0].sParamValue = paymentGatewayName;
            dBOperations[0].sParamType = SqlDbType.VarChar;
            DataTable table = new DataTable();

            string sRetVal = DBOperation.ExecuteDBOperation("GetPaymentGatewayDetails", DBOperation.OperationType.STOREDPROC, dBOperations, ref table);

            if (sRetVal == "SUCCESS")
            {
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        PaymentGatewayDetails paymentGateway = new PaymentGatewayDetails();
                        paymentGateway.ID = Convert.ToInt32(row["ID"]);
                        paymentGateway.PaymentGatewayName = Convert.ToString(row["GatewayName"]);
                        paymentGateway.UserName = Convert.ToString(row["Username"]);
                        paymentGateway.Password = Convert.ToString(row["Password"]);
                        paymentGateway.mailTemplate = new Model.MailTemplate();
                        paymentGateway.mailTemplate.MailSubject = Convert.ToString(row["MailSubject"]);
                        paymentGateway.mailTemplate.MailBody = Convert.ToString(row["MailBody"]);
                        paymentGateway.mailTemplate.MailSignature = Convert.ToString(row["MailSignature"]);
                        gatewayDetails.Add(paymentGateway);
                    }

                }
            }
            return gatewayDetails;
        }
        public DataTable GetRecordForLinkShare()
        {
            //connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);

            DataTable table = new DataTable();
            string sRetVal = DBOperation.ExecuteDBOperation("sp_GetRecordForPaymentLink", DBOperation.OperationType.STOREDPROC, null, ref table);
            if (sRetVal != "SUCCESS")
            {
                logger.LogError(sRetVal);
            }
            return table;
        }

        public void GenerateLink(DataTable table, List<PaymentGatewayDetails> gatewayDetails)
        {
            var tableAsList = table.AsEnumerable();
            Rootobject rootobject = new Rootobject();
            if (gatewayDetails.Count == 1)
            {
                rootobject = PaypalConnect(gatewayDetails[0].UserName, gatewayDetails[0].Password);
            }
            List<string> val = tableAsList.Select(x => x.Field<string>("OrderID")).Distinct().ToList();
            foreach (string orderID in val)
            {
                List<DataRow> dataRows = tableAsList.Where(x => x.Field<string>("OrderID").Equals(orderID)).ToList();
                if (gatewayDetails.Count != 1)
                {
                    rootobject = PaypalConnect(gatewayDetails[0].UserName, gatewayDetails[0].Password);
                }
                RegisterLinkforPayment(orderID, dataRows, rootobject.access_token, gatewayDetails[0]);
            }
        }

        public void RegisterLinkforPayment(string orderID, List<DataRow> rows, string AccessToken, PaymentGatewayDetails gatewayDetails)
        {
            //PaypalDraftRequest.PaypalDraftRequest paypalDraftRequest = new PaypalDraftRequest.PaypalDraftRequest();
            //paypalDraftRequest.detail = new PaypalDraftRequest.Detail();
            //paypalDraftRequest.detail.currency_code = Convert.ToString(row["currency"]);
            //paypalDraftRequest.detail.reference = "Abe Book Reference Number : " + orderID;
            //paypalDraftRequest.detail.note= "Please find herewith the invoice against your order you placed with Abebooks.com.Please check and make payment.If you are facing any problem in making payment please do not hesitate, please contact us at amit@gyanbooks.com or call us on + 91-9899492060";
            //paypalDraftRequest.detail.i


            //paypalDraftRequest.amount = new PaypalDraftRequest.Amount();
            //paypalDraftRequest.amount.breakdown = new PaypalDraftRequest.Breakdown();
            //paypalDraftRequest.amount.breakdown.custom = new PaypalDraftRequest.Custom();
            //paypalDraftRequest.amount.breakdown.custom.amount


            PaypalInovoiceObject paypalInvoiceObject = new PaypalInovoiceObject();
            paypalInvoiceObject.merchant_info = new Merchant_Info();
            paypalInvoiceObject.merchant_info.business_name = ModuleSetings.merchantInfo.BusinessName;
            paypalInvoiceObject.merchant_info.email = ModuleSetings.merchantInfo.email;
            paypalInvoiceObject.merchant_info.first_name = ModuleSetings.merchantInfo.FirstName;
            paypalInvoiceObject.merchant_info.last_name = ModuleSetings.merchantInfo.LastName;
            paypalInvoiceObject.merchant_info.phone = new Phone();
            paypalInvoiceObject.merchant_info.phone.national_number = ModuleSetings.merchantInfo.Phone_Number;
            paypalInvoiceObject.merchant_info.phone.country_code = ModuleSetings.merchantInfo.Phone_CountryCode;

            paypalInvoiceObject.billing_info = new Billing_Info[1];
            paypalInvoiceObject.billing_info[0] = new Billing_Info();
            paypalInvoiceObject.billing_info[0].email = Convert.ToString(rows[0]["email"]);
            paypalInvoiceObject.billing_info[0].first_name = Convert.ToString(rows[0]["CustomerAddress_name"]);

            paypalInvoiceObject.items = new Item[rows.Count];
            int iItemCount = 0;
            foreach (DataRow row in rows)
            {
                paypalInvoiceObject.items[iItemCount] = new Item();
                paypalInvoiceObject.items[iItemCount].name = Convert.ToString(row["title"]);
                paypalInvoiceObject.items[iItemCount].unit_price = new Unit_Price();
                paypalInvoiceObject.items[iItemCount].unit_price.value = Convert.ToString(row["price"]);
                paypalInvoiceObject.items[iItemCount].unit_price.currency = Convert.ToString(row["currency"]);
                paypalInvoiceObject.items[iItemCount].tax = new Tax();
                paypalInvoiceObject.items[iItemCount].tax.name = "TAX";
                paypalInvoiceObject.items[iItemCount].tax.percent = 0;
                paypalInvoiceObject.items[iItemCount].quantity = Convert.ToInt32(row["quantity"]);
                iItemCount++;
            }
            paypalInvoiceObject.shipping_cost = new Shipping_Cost();
            paypalInvoiceObject.shipping_cost.amount = new Amount();
            paypalInvoiceObject.shipping_cost.amount.value = Convert.ToString(rows[0]["orderTotals_shipping_amount"]);
            paypalInvoiceObject.shipping_cost.amount.currency = Convert.ToString(rows[0]["orderTotals_shipping_currency"]);

            paypalInvoiceObject.discount = new Discount();
            paypalInvoiceObject.discount.percent = V;

            paypalInvoiceObject.note = "Please find herewith the invoice against your order you placed with Abebooks.com.Please check and make payment.If you are facing any problem in making payment please do not hesitate, please contact us at amit@gyanbooks.com or call us on + 91-9899492060";
            paypalInvoiceObject.reference = "Abebooks Order-  " + orderID;
            string sRetVal = GetPayPalPaymentID(paypalInvoiceObject, AccessToken);
            if (!string.IsNullOrEmpty(sRetVal))
            {
                string errroInfo = string.Empty;
                InvoiceIdGeneratorResponse.InvoiceIdGeneratorResponse invoiceId = JsonConvert.DeserializeObject<InvoiceIdGeneratorResponse.InvoiceIdGeneratorResponse>(sRetVal);
                if (MarkInvoiceSend(invoiceId.id, AccessToken, ref errroInfo))
                {
                    string invoiceURL = GetPayerInvoiceURL(invoiceId.id, AccessToken);
                    if (string.IsNullOrEmpty(invoiceURL))
                    {
                        if (SendFailureEmail(orderID, "Errorn in getting invoice URL"))
                        {
                            updateDBStatus(orderID, "");
                        }
                    }
                    else
                    {
                        if (SendEmail(invoiceURL, gatewayDetails, rows))
                        {
                            updateDBStatus(orderID, invoiceURL);
                        }
                    }
                }
                else
                {
                    if (SendFailureEmail(orderID, errroInfo))
                    {
                        updateDBStatus(orderID, "");
                    }

                }
            }
        }

        public void updateDBStatus(string orderID, string payerPaymetLnk)
        {
            DataTable dt = new DataTable();
            string sRetVal = string.Empty;
            if (string.IsNullOrEmpty(payerPaymetLnk))
            {
                sRetVal = DBOperation.ExecuteDBOperation("Update PAYMENTLINK SET Status='FAILED',LastModifiedBy=1, LastModifiedON=getdate()  where OrderID= '" + orderID + "'", DBOperation.OperationType.UPDATE, null, ref dt);
            }
            else
            {
                sRetVal = DBOperation.ExecuteDBOperation("Update PAYMENTLINK SET Status='COMPLETED', PayerPaymentLink='" + payerPaymetLnk + "',LastModifiedBy=1, LastModifiedON=getdate()  where OrderID= '" + orderID + "'", DBOperation.OperationType.UPDATE, null, ref dt);
            }
            if (sRetVal != "SUCCESS")
            {
                logger.LogError(sRetVal);
            }

        }
        public bool SendFailureEmail(string orderID, string errroInfo)
        {
            bool result = true;
            MailMessageSettings mailMessageSettings = new MailMessageSettings();
            mailMessageSettings.ToEmailAddress = new List<string>() { Convert.ToString(ModuleSetings.SmptpServer.SmtpEmail) };
            mailMessageSettings.BccEmailAddress = new List<string>() { ModuleSetings.SmptpServer.SmtpEmail };
            mailMessageSettings.EmailSubject = "Paypal Invoice Failure for Order : " + orderID;
            mailMessageSettings.EmailMessage = "Error in marking send for order : " + orderID + "<br/><br/> " + errroInfo + "<br/><br/><br/> Kindly Share it manually.";
            mailMessageSettings.IsBodyHtml = ModuleSetings.Message[0].IsBodyHtml;
            EmailHelper.SendEmailMessage(ModuleSetings.SmptpServer, mailMessageSettings);
            return result;
        }
        public bool SendEmail(string invoiceURL, PaymentGatewayDetails gatewayDetails, List<DataRow> rows)
        {
            bool result = true;
            MailMessageSettings mailMessageSettings = new MailMessageSettings();
            mailMessageSettings.ToEmailAddress = new List<string>() { Convert.ToString(rows[0]["email"]) };
            mailMessageSettings.BccEmailAddress = new List<string>() { ModuleSetings.SmptpServer.SmtpEmail };
            mailMessageSettings.EmailSubject = ReplaceHolder(gatewayDetails.mailTemplate.MailSubject, rows, invoiceURL);
            mailMessageSettings.EmailMessage = ReplaceHolder(gatewayDetails.mailTemplate.MailBody, rows, invoiceURL);
            mailMessageSettings.IsBodyHtml = ModuleSetings.Message[0].IsBodyHtml;
            EmailHelper.SendEmailMessage(ModuleSetings.SmptpServer, mailMessageSettings);
            return result;
        }

        public string ReplaceHolder(string data, List<DataRow> rows, string invoiceURL)
        {
            if (data.Contains(ORDERID_PLACEHOLDER))
            {
                data = data.Replace(ORDERID_PLACEHOLDER, Convert.ToString(rows[0]["OrderID"]));
            }
            if (data.Contains(CLIENTNAME_PLACEHOLDER))
            {
                data = data.Replace(CLIENTNAME_PLACEHOLDER, Convert.ToString(rows[0]["CustomerAddress_name"]));
            }
            if (data.Contains(PAYMENTLINK_PLACEHOLDER))
            {
                data = data.Replace(PAYMENTLINK_PLACEHOLDER, invoiceURL);
            }
            if (data.Contains(ORDERITEMLIST_PLACEHOLDER))
            {
                string itemListData = "<table border=\"1px\" width=\"90 % \">" +
                    "<tr><th width=\"15 % \">Sr. No.</th><th width=\"85 % \">Title</th>";
                int iCount = 1;
                foreach (DataRow row in rows)
                {
                    itemListData += string.Format("<tr><td width=\"15 % \">{0}</td><td width=\"85 % \">{1}</td></tr>", iCount, row["title"]);
                    iCount++;
                }

                itemListData += "</table>";

                data = data.Replace(ORDERITEMLIST_PLACEHOLDER, itemListData);

            }

            return data;
        }

        public string GetPayerInvoiceURL(string InvoiceNumber, string AccessToken)
        {
            string invoiceURL = string.Empty;
            string url = "https://api.paypal.com/v1/invoicing/invoices/{0}";
            HttpResponseMessage HttpResponseMessage = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                    HttpResponseMessage = httpClient.GetAsync(string.Format(url, InvoiceNumber)).Result;
                    if (HttpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        PaypalInvoicePayerURLResponse.PaypalInvoicePayerURLResponse obj = JsonConvert.DeserializeObject<PaypalInvoicePayerURLResponse.PaypalInvoicePayerURLResponse>(HttpResponseMessage.Content.ReadAsStringAsync().Result);
                        invoiceURL = obj.metadata.payer_view_url;
                    }
                    else
                    {
                        logger.LogError("Issue in GetPayerInvoiceURL : " + HttpResponseMessage.StatusCode + ":" + HttpResponseMessage.Content.ToString());
                    }
                }
            }
            catch(Exception e1)
            {
                logger.LogError("Exception in GetPayerInvoiceURL: " + e1.Message + " Stack Trace: " + e1.StackTrace);
            }
            return invoiceURL;
        }

        public bool MarkInvoiceSend(string InvoiceNumber, string AccessToken, ref string ErrorInfo)
        {
            bool result = false;
            try
            {                
                string url = "https://api.paypal.com/v1/invoicing/invoices/{0}/send?notify_merchant=false&notify_customer=true";
                HttpResponseMessage HttpResponseMessage = null;
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                    var myContent = JsonConvert.SerializeObject("");
                    var httpContent = new StringContent(myContent, Encoding.UTF8, "application/json");
                    HttpResponseMessage = httpClient.PostAsync(string.Format(url, InvoiceNumber), httpContent).Result;
                    if (HttpResponseMessage.StatusCode == HttpStatusCode.Accepted)
                    {
                        result = true;
                    }
                    else
                    {
                        ErrorInfo = HttpResponseMessage.Content.ReadAsStringAsync().Result;
                        logger.LogError("Issue in MarkInvoiceSend : " + HttpResponseMessage.StatusCode + ":" + HttpResponseMessage.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, "Exception in MarkInvoiceSend ");
            }

            return result;
        }
        private string GetPayPalPaymentID(PaypalInovoiceObject invoiceObject, string AccessToken)
        {
            string Response = string.Empty;
            HttpResponseMessage HttpResponseMessage = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var myContent = JsonConvert.SerializeObject(invoiceObject);
                var httpContent = new StringContent(myContent, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                HttpResponseMessage = httpClient.PostAsync("https://api.paypal.com/v1/invoicing/invoices", httpContent).Result;
                if (HttpResponseMessage.StatusCode == HttpStatusCode.Created)
                {
                    Response = HttpResponseMessage.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    logger.LogError("Issue in GetPayPalPaymentID : " + HttpResponseMessage.StatusCode + ":" + HttpResponseMessage.Content.ToString());
                }
            }
            return Response;

        }

        public Rootobject PaypalConnect(string UserName, string Password)
        {
            HttpClient client = new HttpClient();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //client.BaseAddress = new Uri("https://api.sandbox.paypal.com/v1/oauth2/");
            client.BaseAddress = new Uri(ModuleSetings.PostURL);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
               $"{UserName}:{Password}")));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>("grant_type", "client_credentials");
            keyValuePairs.Add(keyValuePair);
            var val = client.PostAsync("token", new FormUrlEncodedContent(keyValuePairs)).Result;
            string sData = val.Content.ReadAsStringAsync().Result;
            Rootobject rootobject = JsonConvert.DeserializeObject<Rootobject>(sData);
            return rootobject;
        }
    }
}
