using System;

namespace Infrastructure
{
    [Serializable]
    public class MerchantInfo
    {
        public string email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        public string Phone_CountryCode { get; set; }
        public string Phone_Number { get; set; }
    }
}
