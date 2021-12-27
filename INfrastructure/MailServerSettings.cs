using System;

namespace OPSService.Infrastructure
{
    [Serializable]
    public class MailServerSettings
    {
        #region Server Settings
        public bool EnableSSL { get; set; }

        public string SmtpHost { get; set; }

        public string SmtpEmail { get; set; }

        public string SmtpPassword { get; set; }

        public string SmtpPort { get; set; }
        #endregion

    }
}
