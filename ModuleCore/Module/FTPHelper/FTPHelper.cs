using System.IO;
using System.Net;

namespace ModuleCore.Module.FTPHelper
{
    public static class FTPHelper
    {
        public static string FtpConnect(string ftpHost, string userName, string password)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpHost);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(userName, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string sData = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return sData;
        }

        public static string DownloadFile(string ftpHost, string userName, string password, string FileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpHost + "//" + FileName);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(userName, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string sData = reader.ReadToEnd();

            reader.Close();
            response.Close();
            return sData;
        }
    }
}
