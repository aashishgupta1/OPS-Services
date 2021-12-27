using System;

namespace Infrastructure
{
    [Serializable]
    public class FTPDetails
    {
        public string FtpHost { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
    }
}
