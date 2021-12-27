using OPSService.Infrastructure;
using OPSService.ModuleCore.Interfaces;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceCore
{
    [Serializable]
    public class Settings : ISettings
    {
        public Settings()
        {
            SmptpServer = new MailServerSettings();
            Message = new List<MailMessageSettings>();
        }
        public Encoding DefaultEncoding { get; set; }

        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleAssembly { get; set; }
        public string ModuleStatePath { get; set; }
        public string AssemblyPath { get; set; }
        public MailServerSettings SmptpServer { get; set; }
        public List<MailMessageSettings> Message { get; set; }
        public string Seperator { get; set; }
        public char[] NewLineChars { get; set; }
        public string OutputFileName { get; set; }
        public string DirectoryPath { get; set; }
        public string Pattern { get; set; }
        public string DumpFolderPath { get; set; }
        public string ErrorFolderPath { get; set; }

        /// <summary>
        /// Path2 for errorneous zip/tar files
        /// </summary>
        public string ErrorFolderPath2 { get; set; }
        public bool IsDelete { get; set; }
        public string CronExpression { get; set; }
        public bool StartImmediately { get; set; }
        public string TempFolderPath { get; set; }
        public int MaxQueryCount { get; set; }
        public int? DefaultRetentionPeriod { get; set; }
        public string ReportOutputPath { get; set; }
        public bool UseAppServerDate { get; set; }
        public string LastProcessedDate { get; set; }
        public string GetFormatedFileName(string filename = "")
        {
            if (filename == "")
            {
                filename = OutputFileName;
            }

            if (!string.IsNullOrEmpty(filename))
            {
                var fileParts = filename.Split(new char[] { '{', '}' });
                if (fileParts.Length >= 3)
                {
                    fileParts[1] = DateTime.Now.ToString(fileParts[1].Replace("Y", "y").Replace("D", "d").Replace("S", "s"));
                    return string.Join("", fileParts);
                }
            }
            return filename;
        }
        public string ODBCServerName { get; set; }
        public string ODBCDatabase { get; set; }
        public string ODBCUserName { get; set; }
        public string ODBCPassword { get; set; }

        public string Query { get; set; }
        public string ErrorFileName { get; set; }
        public string PostURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public MerchantInfo merchantInfo { get; set; }
        public FTPDetails FTPDetails { get; set; }
        public string MarketPlaceName { get; set; }
    }
}
