using DBInteraction;
using ModuleCore;
using Newtonsoft.Json;
using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BooksTreasuryUpdate
{
    public class BooksTreasuryUpdateService : ModuleBase
    {
        public const string SUCCESS = "SUCCESS";
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public BooksTreasuryUpdateService(ILogger logger, ISettings settings) : base(logger, settings)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DBOperation.sConnectionString = connectionString;
        }
        public override void Run()
        {
            try
            {
                Update();
            }
            catch (Exception e)
            {
                logger.LogError("Error in updateing Book treasury:" + e.Message + "\n Stack trace :" + e.StackTrace);
            }
        }
        public void Update()
        {
            DateTime Current_date = DateTime.Now;
            DataTable dt = new DataTable();

            string sRetVal = DBOperation.ExecuteDBOperation("select top 1 LastUpdate_date from Update_date", DBOperation.OperationType.SELECT, null, ref dt);
            if (sRetVal == SUCCESS)
            {
                DateTime lastUpdateTime = Convert.ToDateTime(dt.Rows[0][0]);
                dt = new DataTable();
                string sQry = "select PersonalNo, title,creator,imagecount,publisher,isbnno,date, language,price_pb, price, description from BookDataFinalTest " +
                    "where Status='Active' AND LastUpdatedOn>=(CAST(N'" + lastUpdateTime.ToString("yyyy-MM-dd hh:mm:ss") + "' as datetime)) and " +
                    "LastUpdatedOn <(CAST(N'" + Current_date.ToString("yyyy-MM-dd hh:mm:ss") + "' as datetime));";

                sRetVal = DBOperation.ExecuteDBOperation(sQry, DBOperation.OperationType.SELECT, null, ref dt);
                if (sRetVal == SUCCESS)
                {
                    for (int irow = 0; irow < dt.Rows.Count; irow++)
                    {
                        PostAsyncBooksTreasury(Convert.ToString(dt.Rows[irow]["PersonalNo"]), Convert.ToString(dt.Rows[irow]["Title"]), Convert.ToString(dt.Rows[irow]["Creator"]), "Education",
                            Convert.ToString(dt.Rows[irow]["imagecount"]), Convert.ToString(dt.Rows[irow]["Publisher"]), Convert.ToString(dt.Rows[irow]["ISBNNo"]), Convert.ToString(dt.Rows[irow]["Date"]), "Education",
                            Convert.ToString(dt.Rows[irow]["language"]), Convert.ToString(dt.Rows[irow]["Price_PB"]), Convert.ToString(dt.Rows[irow]["Price"]),
                            Convert.ToString(dt.Rows[irow]["Description"]));
                    }
                }
                else
                {
                    logger.LogError(sRetVal);
                }
                sQry = "Update  Update_date set LastUpdate_date=Cast(N'" + Current_date.ToString("yyyy-MM-dd hh:mm:ss") + "' as datetime)";
                sRetVal = DBOperation.ExecuteDBOperation(sQry, DBOperation.OperationType.UPDATE, null, ref dt);
            }
            else
            {
                logger.LogError("Error in fetching last updated date time :" + sRetVal);
            }

        }
        public void PostAsyncBooksTreasury(string sPersonalNo, string sTITLE, string sCREATOR, string sCATEGORY, string sIMAGECOUNT, string sPUBLISHER,
            string sISBN_04, string sDATE, string sBOOKCAT, string sLANGUAGE, string sINR_PB, string sINR, string sDESCRIPTION)
        {
            #region Test to consume web api
            #region For Proxy
            //string proxyUri = string.Format("{0}:{1}", "192.168.55.218", "8080");

            //NetworkCredential proxyCreds = new NetworkCredential("aashish.gupta", "Oct@2017");

            //WebProxy proxy = new WebProxy(proxyUri, false)
            //{
            //    UseDefaultCredentials = false,
            //    Credentials = proxyCreds,
            //};

            //// Now create a client handler which uses that proxy
            //HttpClientHandler httpClientHandler = new HttpClientHandler()
            //{
            //    Proxy = proxy,
            //    PreAuthenticate = true,
            //    UseDefaultCredentials = false,
            //};

            //// You only need this part if the server wants a username and password:
            //string httpUserName = "aashish.gupta", httpPassword = "Oct@2017";

            //httpClientHandler.Credentials = new NetworkCredential(httpUserName, httpPassword);
            #endregion

            //HttpClient client = new HttpClient(httpClientHandler);
            HttpClient client = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(sTITLE), "TITLE");
            form.Add(new StringContent(sCREATOR), "CREATOR");
            form.Add(new StringContent(sCATEGORY), "CATEGORY");
            form.Add(new StringContent(sIMAGECOUNT), "IMAGECOUNT");
            form.Add(new StringContent(sPUBLISHER), "PUBLISHER");
            form.Add(new StringContent(sISBN_04), "ISBN_04");
            form.Add(new StringContent(sDATE), "DATE");
            form.Add(new StringContent(sBOOKCAT), "BOOKCAT");
            form.Add(new StringContent(sLANGUAGE), "LANGUAGE");
            form.Add(new StringContent(sINR_PB), "INR_PB");
            form.Add(new StringContent(sINR), "INR");
            form.Add(new StringContent(sDESCRIPTION), "DESCRIPTION");
            form.Add(new StringContent(sPersonalNo), "PERSONALNO");
            form.Add(new StringContent("true"), "bookstreasuryIntegeration");
            object data = new
            {
                TITLE = sTITLE,//"ffNHsgrs123yyppaa"
                CREATOR = sCREATOR,
                CATEGORY = sCATEGORY,
                IMAGECOUNT = sIMAGECOUNT,
                PUBLISHER = sPUBLISHER,
                ISBN_04 = sISBN_04,
                DATE = sDATE,
                BOOKCAT = sBOOKCAT,
                LANGUAGE = sLANGUAGE,
                INR_PB = sINR_PB,
                INR = sINR,
                DESCRIPTION = sDESCRIPTION,
                PERSONALNO = sPersonalNo

            };
            var myContent = JsonConvert.SerializeObject(data);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            HttpResponseMessage response = client.PostAsync("http://www.bookstreasury.com/index.php/api2/inventory/add", form).Result;
            if (response.IsSuccessStatusCode)
            {
                logger.LogDebug($"Response for Personla Number ({sPersonalNo}) " + response.Content.ReadAsStringAsync().Result);
                //Logs.StoreActivityLogsInDB("PostAsyncBooksTreasury", GlobalData.iUserID, response.IsSuccessStatusCode + "For Personal Number : " + sPersonalNo + "  ISBN No :" + sISBN_04 + "  Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase, "Global Data", "PostAsyncBooksTreasurySuccess");
            }
            else
            {
                logger.LogError($"Response for Personla Number ({sPersonalNo}) " + response.IsSuccessStatusCode);
                //Logs.StoreActivityLogsInDB("PostAsyncBooksTreasury", GlobalData.iUserID, response.IsSuccessStatusCode + "For Personal Number : " + sPersonalNo + "  ISBN No :" + sISBN_04 + "  Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase, "Global Data", "PostAsyncBooksTreasury");
                //MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
            #endregion
        }


    }
}
