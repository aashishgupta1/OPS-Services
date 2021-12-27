
using System;
namespace Bonanza_Order_Fetch.Model
{
    public class Rootobject
    {
        public string ack { get; set; }
        public string version { get; set; }
        public DateTime timestamp { get; set; }
        public Fetchtokenresponse fetchTokenResponse { get; set; }
    }

    public class Fetchtokenresponse
    {
        public string authToken { get; set; }
        public DateTime hardExpirationTime { get; set; }
        public string authenticationURL { get; set; }
    }
}