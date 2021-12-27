namespace BookTreasury_Order_Fetch
{
    public class OrderResponse
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string order_no { get; set; }
        public string order_date { get; set; }
        public Customer customer { get; set; }
        public float sub_total { get; set; }
        public string gift { get; set; }
        public string box { get; set; }
        public string coupon_id { get; set; }
        public float? coupon_amount { get; set; }
        public string shipping_type { get; set; }
        public int? shipping_charges { get; set; }
        public float total { get; set; }
        public string currency { get; set; }
        public int? referral_id { get; set; }
        public string referral_amount { get; set; }
        public string status { get; set; }
        public string created_on { get; set; }
        public Order_Items[] order_items { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string full_name { get; set; }
        public string address { get; set; }
        public string town_city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postal_zip_code { get; set; }
        public string shipping_full_name { get; set; }
        public string shipping_address { get; set; }
        public string shipping_town_city { get; set; }
        public string shipping_state { get; set; }
        public string shipping_country { get; set; }
        public string shipping_postal_zip_code { get; set; }
        public long? shipping_phone { get; set; }
        public string order_notes { get; set; }
    }

    public class Order_Items
    {
        public int id { get; set; }
        public int? address_id { get; set; }
        public int order_id { get; set; }
        public string book_id { get; set; }
        public float price { get; set; }
        public int quantity { get; set; }
        public string processing_fee { get; set; }
        public int has_attributes { get; set; }
        public int status { get; set; }
    }

}
