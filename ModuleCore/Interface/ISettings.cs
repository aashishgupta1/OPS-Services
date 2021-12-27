
using OPSService.Infrastructure;
using Infrastructure;
using System.Collections.Generic;
using System.Text;

namespace OPSService.ModuleCore.Interfaces
{
    public interface ISettings
    {
        Encoding DefaultEncoding { get; set; }

        int ModuleId { get; set; }
        string ModuleName { get; set; }
        string ModuleStatePath { get; set; }
        string ModuleAssembly { get; set; }
        string AssemblyPath { get; set; }

        MailServerSettings SmptpServer { get; set; }

        List<MailMessageSettings> Message { get; set; }

        string Seperator { get; set; }
        char[] NewLineChars { get; set; }

        string DirectoryPath { get; set; }
        string Pattern { get; set; }
        string OutputFileName { get; set; }
        string DumpFolderPath { get; set; }
        string ErrorFolderPath { get; set; }
        string ErrorFolderPath2 { get; set; }
        string CronExpression { get; set; }
        bool StartImmediately { get; set; }
        bool IsDelete { get; set; }
        string TempFolderPath { get; set; }
        int MaxQueryCount { get; set; }
        string GetFormatedFileName(string filename = "");
        bool UseAppServerDate { get; set; }
        int? DefaultRetentionPeriod { get; set; }
        string LastProcessedDate { get; set; }
        string ReportOutputPath { get; set; }
        string ODBCServerName { get; set; }
        string ODBCDatabase { get; set; }
        string ODBCUserName { get; set; }
        string ODBCPassword { get; set; }
        string Query { get; set; }
        string ErrorFileName { get; set; }
        string PostURL { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        MerchantInfo merchantInfo { get; set; }
        FTPDetails FTPDetails { get; set; }
        string MarketPlaceName { get; set; }
    }
}
