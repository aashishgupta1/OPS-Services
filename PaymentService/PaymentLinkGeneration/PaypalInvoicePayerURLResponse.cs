namespace PaypalInvoicePayerURLResponse
{
    public class PaypalInvoicePayerURLResponse
    {
        public string id { get; set; }
        public string number { get; set; }
        public string template_id { get; set; }
        public string status { get; set; }
        public Merchant_Info merchant_info { get; set; }
        public Billing_Info[] billing_info { get; set; }
        public Item[] items { get; set; }
        public string invoice_date { get; set; }
        public Discount discount { get; set; }
        public Shipping_Cost shipping_cost { get; set; }
        public bool tax_calculated_after_discount { get; set; }
        public bool tax_inclusive { get; set; }
        public string terms { get; set; }
        public string note { get; set; }
        public Total_Amount total_amount { get; set; }
        public Metadata metadata { get; set; }
        public bool allow_tip { get; set; }
        public Link[] links { get; set; }
    }

    public class Merchant_Info
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string business_name { get; set; }
        public Phone phone { get; set; }
        public Address address { get; set; }
    }

    public class Phone
    {
        public string country_code { get; set; }
        public string national_number { get; set; }
    }

    public class Address
    {
        public Phone1 phone { get; set; }
    }

    public class Phone1
    {
        public string country_code { get; set; }
        public string national_number { get; set; }
    }

    public class Discount
    {
        public float percent { get; set; }
        public Amount amount { get; set; }
    }

    public class Amount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Shipping_Cost
    {
        public Amount1 amount { get; set; }
    }

    public class Amount1
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Total_Amount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Metadata
    {
        public string created_date { get; set; }
        public string payer_view_url { get; set; }
    }

    public class Billing_Info
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public float quantity { get; set; }
        public Unit_Price unit_price { get; set; }
        public Tax tax { get; set; }
    }

    public class Unit_Price
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public string name { get; set; }
        public float percent { get; set; }
        public Amount2 amount { get; set; }
    }

    public class Amount2
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string method { get; set; }
    }
}