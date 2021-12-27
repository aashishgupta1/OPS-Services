public class PaypalInovoiceObject
{
    public Merchant_Info merchant_info { get; set; }
    public Billing_Info[] billing_info { get; set; }
    public Item[] items { get; set; }
    public Discount discount { get; set; }
    public Shipping_Cost shipping_cost { get; set; }
    public string note { get; set; }
    public string terms { get; set; }
    public string reference { get; set; }
}

public class Merchant_Info
{
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string business_name { get; set; }
    public Phone phone { get; set; }
}

public class Phone
{
    public string country_code { get; set; }
    public string national_number { get; set; }
}

public class Discount
{
    public decimal percent { get; set; }
}

public class Shipping_Cost
{
    public Amount amount { get; set; }
}

public class Amount
{
    public string currency { get; set; }
    public string value { get; set; }
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
    public int quantity { get; set; }
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
    public int percent { get; set; }
}
