using OPSService.Infrastructure;
using OPSService.Infrastructure.Loggging;
using Infrastructure;
using ModuleCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace ServiceCore
{
    public static class SettingHandler
    {
        #region Fields
        private readonly static string encryptionKey = "$canViewOneDM$Job$erver";

        /// <summary>
        /// Enable SSL
        /// </summary>
        private const string enableSSLKey = "EnableSSL";

        /// <summary>
        /// The temporary folder path key
        /// </summary>
        private const string tempFolderPathKey = "TempFolderPath";

        /// <summary>
        /// The encoding key
        /// </summary>
        private const string encodingKey = "Encoding";

        private const string AFPToPDFConverterHostKey = "AFPToPDFConverterHost";

        private const string AFPToPDFConverterPortKey = "AFPToPDFConverterPort";

        private const string TEMP = "Temp";

        /// <summary>
        /// The start job immediately
        /// </summary>
        private const string startJobImmediatelyKey = "StartJobImmediately";
        #endregion

        /// <summary>
        /// Load settings.
        /// </summary>
        /// <param name="settingFilePath">The setting file path.</param>
        /// <returns></returns>
        public static Settings LoadSettings(string settingFilePath)
        {
            XPathDocument xmlDoc = new XPathDocument(settingFilePath);
            Settings moduleSetting = new Settings();
            XPathNavigator navigator = xmlDoc.CreateNavigator();

            moduleSetting.ModuleId = Convert.ToInt32(GetConfigValue(navigator, "/Module/ModuleId"));
            moduleSetting.ModuleName = GetConfigValue(navigator, "/Module/ModuleName");
            moduleSetting.ModuleAssembly = GetConfigValue(navigator, "/Module/ModuleAssembly");
            moduleSetting.ModuleStatePath = GetConfigValue(navigator, "/Module/ModuleStatePath");

            string encoding = ConfigurationManager.AppSettings[encodingKey];
            if (!string.IsNullOrEmpty(encoding))
            {
                moduleSetting.DefaultEncoding = Encoding.GetEncoding(encoding);
            }
            else
            {
                moduleSetting.DefaultEncoding = Encoding.UTF8;
            }

            moduleSetting.PostURL = GetConfigValue(navigator, "/Module/ApIURL");
            moduleSetting.Username = GetConfigValue(navigator, "/Module/UserName");
            moduleSetting.Password = GetConfigValue(navigator, "/Module/Password");
            moduleSetting.Seperator = GetConfigValue(navigator, "/Module/Seperator");
            moduleSetting.NewLineChars = LoadNewLineChars(navigator, "/Module/NewLineChar/CharCode");

            LoadEmailSettings(moduleSetting, navigator);
            moduleSetting.DirectoryPath = GetConfigValue(navigator, "/Module/FileSystemSettings/InputPath/Path");
            string searchPattern = GetConfigValue(navigator, "/Module/FileSystemSettings/InputPath/Pattern");
            moduleSetting.Pattern = string.IsNullOrWhiteSpace(searchPattern) ? "*.*" : searchPattern;

            moduleSetting.DumpFolderPath = GetConfigValue(navigator, "/Module/FileSystemSettings/OutputPath/Path");
            moduleSetting.OutputFileName = GetConfigValue(navigator, "/Module/FileSystemSettings/OutputPath/FileName");


            bool.TryParse(GetConfigValue(navigator, "/Module/FileSystemSettings/OutputPath/IsDelete"), out bool isDelete);
            moduleSetting.IsDelete = isDelete;
            bool.TryParse(GetConfigValue(navigator, "/Module/ClearListBeforeImport"), out bool clearList);

            bool.TryParse(GetConfigValue(navigator, "/Module/UseAppServerDate"), out bool useDate);
            moduleSetting.UseAppServerDate = useDate;
            moduleSetting.ErrorFolderPath = GetConfigValue(navigator, "/Module/FileSystemSettings/ErrorPath/Path");
            moduleSetting.ErrorFolderPath2 = GetConfigValue(navigator, "/Module/FileSystemSettings/ErrorPath/Path2");
            moduleSetting.ReportOutputPath = GetConfigValue(navigator, "/Module/FileSystemSettings/ReportOutputPath/Path");
            moduleSetting.CronExpression = GetConfigValue(navigator, "/Module/CronExpression");
            int.TryParse(GetConfigValue(navigator, "/Module/ThreadCount"), out int threadCount);

            int.TryParse(GetConfigValue(navigator, "/Module/MaxQueryCount"), out int maxQueryCount);
            moduleSetting.MaxQueryCount = maxQueryCount;

            int.TryParse(GetConfigValue(navigator, "/Module/ForceItemType"), out int forceItemType);
            string startJobImmediately = ConfigurationManager.AppSettings[startJobImmediatelyKey];
            if (startJobImmediately.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                moduleSetting.StartImmediately = true;
            }
            string tempFolderPath = ConfigurationManager.AppSettings[tempFolderPathKey];
            if (!string.IsNullOrWhiteSpace(tempFolderPath))
            {
                moduleSetting.TempFolderPath = tempFolderPath;
            }
            else
            {
                moduleSetting.TempFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), TEMP);
            }
            if (!Directory.Exists(moduleSetting.TempFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(moduleSetting.TempFolderPath);
                }
                catch (Exception)
                {
                    Logger.Instance.LogInfo("Unable to create temp folder directory");
                }
            }

            if (int.TryParse(GetConfigValue(navigator, "/Module/DefaultRetentionPeriod"), out int defaultRetentionDays))
            {
                moduleSetting.DefaultRetentionPeriod = defaultRetentionDays;
            }
            moduleSetting.LastProcessedDate = GetConfigValueWithNull(navigator, "/Module/LastProcessedDate");
            moduleSetting.MarketPlaceName = GetConfigValueWithNull(navigator, "/Module/MarketPlaceName");

            LoadODBCDetails(navigator, moduleSetting);
            moduleSetting.Query = GetConfigValue(navigator, "/Module/ODBCSetting/Query");
            moduleSetting.ErrorFileName = GetConfigValue(navigator, "/Module/FileSystemSettings/ErrorPath/FileName");

            moduleSetting.merchantInfo = new MerchantInfo();
            moduleSetting.merchantInfo = LoadMerchantDetails(navigator);

            moduleSetting.FTPDetails = new FTPDetails();
            moduleSetting.FTPDetails = LoadFTPDetails(navigator);

            return moduleSetting;
        }

        public static FTPDetails LoadFTPDetails(XPathNavigator navigator)
        {
            FTPDetails fTPDetails = new FTPDetails();
            fTPDetails.FtpHost = GetConfigValue(navigator, "Module/FTPDetails/FTPHost");
            fTPDetails.FtpUserName = GetConfigValue(navigator, "Module/FTPDetails/UserName");
            fTPDetails.FtpPassword = GetConfigValue(navigator, "Module/FTPDetails/Password");
            return fTPDetails;
        }
        public static MerchantInfo LoadMerchantDetails(XPathNavigator navigator)
        {
            MerchantInfo merchantInfo = new MerchantInfo();
            merchantInfo.email = GetConfigValue(navigator, "Module/MerchantInfo/email");
            merchantInfo.LastName = GetConfigValue(navigator, "Module/MerchantInfo/last_name");
            merchantInfo.FirstName = GetConfigValue(navigator, "Module/MerchantInfo/first_name");
            merchantInfo.BusinessName = GetConfigValue(navigator, "Module/MerchantInfo/business_name");
            merchantInfo.Phone_CountryCode = GetConfigValue(navigator, "Module/MerchantInfo/phone/country_code");
            merchantInfo.Phone_Number = GetConfigValue(navigator, "Module/MerchantInfo/phone/national_number");
            return merchantInfo;
        }
        public static void LoadODBCDetails(XPathNavigator navigator, Settings setting)
        {
            string inheritODBCSettings = GetConfigValue(navigator, "/Module/ODBCSetting/InheritConfiguration");
            bool bLoaded = false;
            if (inheritODBCSettings.Equals("True", StringComparison.InvariantCultureIgnoreCase) || inheritODBCSettings == "1")
            {
                string baseSettingXmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Setting.xml");
                if (File.Exists(baseSettingXmlPath))
                {
                    XPathDocument xmlBaseDoc = new XPathDocument(baseSettingXmlPath);
                    XPathNavigator navigatorBase = xmlBaseDoc.CreateNavigator();

                    setting.ODBCServerName = GetConfigValue(navigatorBase, "/Module/ODBCSetting/DatabaseServer");
                    setting.ODBCDatabase = GetConfigValue(navigatorBase, "/Module/ODBCSetting/Database");
                    setting.ODBCUserName = GetConfigValue(navigatorBase, "/Module/ODBCSetting/DatabaseUserName");
                    setting.ODBCPassword = GetConfigValue(navigatorBase, "/Module/ODBCSetting/DatabasePassword");
                    bLoaded = true;
                }
            }
            if (!bLoaded)
            {
                setting.ODBCServerName = GetConfigValue(navigator, "/Module/ODBCSetting/DatabaseServer");
                setting.ODBCDatabase = GetConfigValue(navigator, "/Module/ODBCSetting/Database");
                setting.ODBCUserName = GetConfigValue(navigator, "/Module/ODBCSetting/DatabaseUserName");
                setting.ODBCPassword = GetConfigValue(navigator, "/Module/ODBCSetting/DatabasePassword");

            }

        }
        public static List<OutputDocSetting> LoadOutputDocSetting(XPathNavigator navigator)
        {
            List<OutputDocSetting> result = new List<OutputDocSetting>();
            //string docSetting = GetConfigValue(navigator, "/Module/FileSystemSettings/OutputPath/DocSettings");
            XPathNodeIterator xPathNodeIterator = navigator.Select("/Module/FileSystemSettings/OutputPath/DocSettings/DocSetting");
            while (xPathNodeIterator.MoveNext())
            {
                OutputDocSetting outputDocSetting = new OutputDocSetting
                {
                    DocType = GetConfigValue(xPathNodeIterator.Current, "DocType"),
                    MaxSize = long.Parse(GetConfigValue(xPathNodeIterator.Current, "MaxSize"))
                };
                result.Add(outputDocSetting);
            }
            return result;
        }
        public static List<string> LoadOutputPaths(XPathNavigator navigator)
        {
            List<string> result = new List<string>();
            XPathNodeIterator xPathNodeIterator = navigator.Select("/Module/FileSystemSettings/OutputPath/UnzipFilePaths/Path");
            string tempPath = "";
            while (xPathNodeIterator.MoveNext())
            {
                tempPath = xPathNodeIterator.Current.Value;
                result.Add(tempPath);
            }
            return result;
        }


        /// <summary>
        /// Loads the new line chars.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <param name="IndexType">Type of the index.</param>
        /// <returns></returns>
        public static char[] LoadNewLineChars(XPathNavigator navigator, string IndexType)
        {
            XPathNodeIterator iterator = navigator.Select(IndexType);
            var chars = new List<char>();
            while (iterator.MoveNext())
            {
                if (int.TryParse(iterator.Current.Value, out int charCode))
                {
                    chars.Add((char)charCode);
                }
            }
            if (chars.Count == 0)
            {
                chars.Add((char)13);
            }
            return chars.ToArray();
        }


        public static void LoadEmailSettings(Settings moduleSetting, XPathNavigator navigator)
        {
            string baseSettingXmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Setting.xml");
            if (File.Exists(baseSettingXmlPath))
            {
                XPathDocument xmlBaseDoc = new XPathDocument(baseSettingXmlPath);
                XPathNavigator navigatorBase = xmlBaseDoc.CreateNavigator();
                moduleSetting.SmptpServer.SmtpEmail = GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpEmail");
                moduleSetting.SmptpServer.SmtpHost = GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpHost");
                moduleSetting.SmptpServer.SmtpPassword = GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpPassword");
                string isEncrypted = GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpPassword/@IsEncrypted");
                if (bool.TryParse(isEncrypted, out bool isEncryptedValue) && isEncryptedValue)
                {
                    moduleSetting.SmptpServer.SmtpPassword = DecryptPWD(GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpPassword"));
                }
                else
                {
                    moduleSetting.SmptpServer.SmtpPassword = GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpPassword");
                    EncryptSMTPPWD(baseSettingXmlPath);
                }

                moduleSetting.SmptpServer.SmtpPort = GetConfigValue(navigatorBase, "/Module/MailServerSettings/SmtpPort");
                if (bool.TryParse(GetConfigValue(navigatorBase, "/Module/MailServerSettings/EnableSSL"), out bool enableSSL))
                {
                    moduleSetting.SmptpServer.EnableSSL = enableSSL;
                }
                else
                {
                    Logger.Instance.LogWarn("Unable to parse EnableSSL appsetting value");
                }

            }

            XPathNodeIterator iterator = navigator.Select("/Module/Emails/Email");
            List<MailMessageSettings> messageMail = new List<MailMessageSettings>();
            if (iterator.Count > 0)
            {
                while (iterator.MoveNext())
                {
                    MailMessageSettings messageMailValues = new MailMessageSettings();
                    if (GetConfigValue(iterator.Current, "IsBodyHtml").Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        messageMailValues.IsBodyHtml = true;
                    }
                    else
                    {
                        messageMailValues.IsBodyHtml = false;
                    }

                    var toAddr = GetConfigValue(iterator.Current, "To").Split(';', ',');
                    messageMailValues.ToEmailAddress = new List<string>();
                    foreach (var item in toAddr)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            messageMailValues.ToEmailAddress.Add(item);
                        }
                    }

                    var ccAddr = GetConfigValue(iterator.Current, "CC").Split(';', ',');
                    messageMailValues.CcEmailAddress = new List<string>();
                    foreach (var item in ccAddr)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            messageMailValues.CcEmailAddress.Add(item);
                        }
                    }

                    var bccAddr = GetConfigValue(iterator.Current, "BCC").Split(';', ',');
                    messageMailValues.BccEmailAddress = new List<string>();
                    foreach (var item in bccAddr)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            messageMailValues.BccEmailAddress.Add(item);
                        }
                    }
                    messageMailValues.EmailSubject = GetConfigValue(iterator.Current, "Subject");
                    messageMailValues.EmailMessage = GetConfigValue(iterator.Current, "Body");
                    messageMailValues.Key = GetConfigValue(iterator.Current, "Key");
                    messageMail.Add(messageMailValues);
                }
            }
            moduleSetting.Message = messageMail;
        }
        private static string LoadPassword(Settings moduleSetting, XPathNavigator navigator, string passwordPath, string settingPath)
        {
            string isEncrypted = GetConfigValue(navigator, passwordPath + "/@IsEncrypted");
            string password = string.Empty;
            if (bool.TryParse(isEncrypted, out bool isEncryptedValue) && isEncryptedValue)
            {
                if (isEncryptedValue)
                {
                    password = DecryptPWD(GetConfigValue(navigator, passwordPath));
                }
            }
            else
            {
                password = GetConfigValue(navigator, passwordPath);
                EncryptPWD(settingPath, passwordPath);
            }
            return password;
        }


        /// <summary>
        /// Private helper method. Returns the config value for the
        /// given XPath.
        /// </summary>
        /// <param name="navigator">The navigator to use.</param>
        /// <param name="xpath">The XPath.</param>
        /// <param name="trim">Optional. If true, value is trimmed. Default is true.</param>
        /// <returns>
        /// The value.
        /// </returns>
        internal static string GetConfigValue(XPathNavigator navigator, string xpath, bool trim = true)
        {
            XPathNavigator valNav = navigator.SelectSingleNode(xpath);
            if (valNav != null)
            {
                return trim ? valNav.Value.Trim() : valNav.Value;
            }
            return string.Empty;
        }

        internal static string GetConfigValueWithNull(XPathNavigator navigator, string xpath, bool trim = true)
        {
            XPathNavigator valNav = navigator.SelectSingleNode(xpath);
            if (valNav == null)
            {
                return null;
            }
            else
            {
                return trim ? valNav.Value.Trim() : valNav.Value;
            }
        }

        internal static int GetNumericConfigValue(XPathNavigator navigator, string xpath, bool trim = true)
        {
            XPathNavigator valNav = navigator.SelectSingleNode(xpath);
            if (valNav != null)
            {
                string value = trim ? valNav.Value.Trim() : valNav.Value;
                int.TryParse(value, out int numericValue);
                return numericValue;
            }
            return 0;
        }


        /// <summary>
        /// Load regex.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <returns></returns>
        private static List<string> LoadRegex(XPathNavigator navigator)
        {
            return GetListConfigValues(navigator, "RegularExpressions/Regex");
        }

        private static List<string> LoadTLEValues(XPathNavigator navigator)
        {
            return GetListConfigValues(navigator, "TLEValues/TLEValue");
        }

        private static List<string> LoadXMPValues(XPathNavigator navigator)
        {
            return GetListConfigValues(navigator, "XMPValues/XMPValue");
        }

        private static List<string> GetListConfigValues(XPathNavigator navigator, string path)
        {
            List<string> values = new List<string>();
            XPathNodeIterator iterator = navigator.Select(path);
            while (iterator.MoveNext())
            {
                values.Add(iterator.Current.Value);
            }
            return values;
        }

        private static List<string> LoadSearchKeywords(XPathNavigator navigator)
        {
            return GetListConfigValues(navigator, "SearchKeywords/Keyword");
        }


        /// <summary>
        /// Load Group Replaces
        /// </summary>
        /// <param name="navigator">The Navigator</param>
        /// <returns></returns>
        private static Dictionary<string, string> LoadGroupReplaces(XPathNavigator navigator)
        {
            Dictionary<string, string> groupReplaces = new Dictionary<string, string>();
            XPathNodeIterator iterator = navigator.Select("GroupReplaces/GroupReplace");
            while (iterator.MoveNext())
            {
                groupReplaces.Add(iterator.Current.SelectSingleNode("Key").Value, iterator.Current.SelectSingleNode("Value").Value);
            }
            return groupReplaces;
        }

        /// <summary>
        /// Encrypts the password.
        /// </summary>
        /// <param name="settingFilePath">The setting file path.</param>
        private static void EncryptPWD(string settingFilePath, string passwordPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@settingFilePath);

            // get a list of nodes - in this case, I'm selecting all <AID> nodes under
            // the <GroupAIDs> node - change to suit your needs
            XmlNode node = doc.SelectSingleNode(passwordPath);

            if (node != null)
            {
                XmlAttribute attr = node.Attributes["IsEncrypted"];
                if (attr == null)
                {
                    attr = doc.CreateAttribute("IsEncrypted");
                    node.Attributes.Append(attr);
                }

                // Need to ad Encryption Code
                //using (Des3Crypto des3Crypto = new Des3Crypto(encryptionKey))
                //{
                //    node.InnerText = des3Crypto.Encrypt(node.InnerText);
                //}

                node.InnerText = node.InnerText;
                attr.Value = "True";

                doc.Save(@settingFilePath);
            }
        }


        private static void EncryptSMTPPWD(string settingFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@settingFilePath);

            XmlNode node = doc.SelectSingleNode("/Module/MailServerSettings/SmtpPassword");

            if (node != null)
            {
                XmlAttribute attr = node.Attributes["IsEncrypted"];
                if (attr == null)
                {
                    attr = doc.CreateAttribute("IsEncrypted");
                    node.Attributes.Append(attr);
                }

                //Need to Add Encryption Code
                //using (Des3Crypto des3Crypto = new Des3Crypto(encryptionKey))
                //{
                //    node.InnerText = des3Crypto.Encrypt(node.InnerText);
                //}
                node.InnerText = node.InnerText;
                attr.Value = "True";

                doc.Save(@settingFilePath);
            }
        }

        /// <summary>
        /// Decrypts the password.
        /// </summary>
        /// <param name="encryptedPWD">The encrypted password.</param>
        /// <returns></returns>
        private static string DecryptPWD(string encryptedPWD)
        {
            string plainPWD = encryptedPWD;
            //eed to add decryption Code
            //using (Des3Crypto des3Crypto = new Des3Crypto(encryptionKey))
            //{
            //    plainPWD = des3Crypto.Decrypt(encryptedPWD);
            //}
            plainPWD = encryptedPWD;
            return plainPWD;
        }
    }
}
