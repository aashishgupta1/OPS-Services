using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using ModuleCore.Module.FTPHelper;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DBProject;
using ModuleCore.Module.OrderHelper;

namespace Alibris_OrderFetch
{
    class AlibrisOrderFetch : ModuleBase
    {
        string userName = string.Empty;
        string password = string.Empty;
        private bool autoFetch = false;
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public AlibrisOrderFetch(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
            
        }

        public bool GetAlibrisDetails()
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
            if (GetAlibrisDetails())
            {
                if (autoFetch)
                    ProcessOrder();
            }
        }
        private void ProcessOrder()
        {
            string sFileName = FTPHelper.FtpConnect(ModuleSetings.FTPDetails.FtpHost, userName, password);
            string[] delimiter = new string[] { "\r", "\n" };
            string[] sFileList = sFileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] sProcessFileList = sFileList.Where(x => !File.Exists(Path.Combine(ModuleSetings.DumpFolderPath, x))).ToArray();
            foreach (string sFile in sProcessFileList)
            {
                string sFileData = ProcessEachFile(sFile);
                ReadTabFile(sFileData);
            }

        }
        private string ProcessEachFile(string sFileName)
        {
            string sFileData = FTPHelper.DownloadFile(ModuleSetings.FTPDetails.FtpHost, userName, password, sFileName);
            StreamWriter sw = new StreamWriter(Path.Combine(ModuleSetings.DumpFolderPath, sFileName));
            sw.Write(sFileData);
            sw.Close();

            return sFileData;
        }
        private void ReadTabFile(string sFileData)
        {
            string[] delimitter = new string[] { "\r", "\n" };
            string[] sLineData = sFileData.Split(delimitter, StringSplitOptions.RemoveEmptyEntries);
            string[] lineDataDelim = new string[] { "\t" };
            List<Order_Detail> lstorder_Detail = new List<Order_Detail>();
            Order_Detail objorder_Detail = new Order_Detail();
            Order_Other_Detail objorder_Other_Detail = new Order_Other_Detail();

            for (int iRowCout = 1; iRowCout < sLineData.Count(); iRowCout++)
            {
                string[] rowData = sLineData[iRowCout].Split(lineDataDelim, StringSplitOptions.None);
                string OrderID = rowData[1];
                if (OrderHelper.GetOrderDetailsByOrderID(OrderID).Count() == 0)
                {
                    int iIndex = lstorder_Detail.FindIndex(x => x.Order_id == OrderID);
                    if (iIndex == -1)
                    {
                        ORDER_ITEM_DETAIL item_detail = new ORDER_ITEM_DETAIL()
                        {
                            OrderItemId = rowData[2],
                            description = rowData[5],
                            ItemCode = rowData[7],
                            ItemPrice = Convert.ToInt32(rowData[8]),
                            Quantity = Convert.ToInt32(rowData[10]),

                        };
                        lstorder_Detail[iIndex].ORDER_ITEM_DETAIL.Add(item_detail);
                        lstorder_Detail[iIndex].Order_Quantity += Convert.ToInt32(item_detail.Quantity);
                    }
                    else
                    {
                        objorder_Detail = new Order_Detail()
                        {
                            payment_Status = rowData[0],
                            Order_id = OrderID,
                            Payment_Date = string.IsNullOrEmpty(rowData[3]) ? DateTime.MaxValue : Convert.ToDateTime(rowData[3]),
                            shipping = Convert.ToInt32(rowData[9]),
                            Total_Amount = Convert.ToInt32(rowData[11]),
                            PurchaseDate = string.IsNullOrEmpty(rowData[12]) ? DateTime.MaxValue : Convert.ToDateTime(rowData[12]),
                            BuyerEmail = rowData[14],
                            BuyerName = rowData[15],
                            Name = rowData[16],
                            AddressLine1 = rowData[17],
                            AddressLine2 = rowData[18],
                            City = rowData[19],
                            StateOrRegion = rowData[20],
                            PostalCode = rowData[21],
                            Country = rowData[22],
                            Ship_Method = rowData[25],
                        };

                        objorder_Other_Detail = new Order_Other_Detail()
                        {
                            payments_transaction_id = rowData[4],
                            batch_id = rowData[13],
                            remarks = rowData[23]
                            //upc = rowData[24],
                        };
                        objorder_Detail.Order_Other_Detail.Add(objorder_Other_Detail);


                        ORDER_ITEM_DETAIL item_detail = new ORDER_ITEM_DETAIL()
                        {
                            OrderItemId = rowData[2],
                            description = rowData[5],
                            ItemCode = rowData[7],
                            ItemPrice = Convert.ToInt32(rowData[8]),
                            Quantity = Convert.ToInt32(rowData[10]),

                        };
                        objorder_Detail.Order_Quantity += item_detail.Quantity;

                        objorder_Detail.ORDER_ITEM_DETAIL.Add(item_detail);

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
