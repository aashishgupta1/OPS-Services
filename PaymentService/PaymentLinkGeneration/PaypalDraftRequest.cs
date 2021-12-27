namespace PaymentLinkGeneration.PaypalDraftRequest
{

    public class PaypalDraftRequest
    {
        public Detail detail { get; set; }
        public Invoicer invoicer { get; set; }
        public Primary_Recipients[] primary_recipients { get; set; }
        public Item[] items { get; set; }
        public Configuration configuration { get; set; }
        public Amount amount { get; set; }
        public string invoice_date { get; set; }
    }

    public class Detail
    {
        public string invoice_number { get; set; }
        public string reference { get; set; }
        public string currency_code { get; set; }
        public string note { get; set; }
        public string term { get; set; }
        public string memo { get; set; }
        public Payment_Term payment_term { get; set; }
    }

    public class Payment_Term
    {
        public string term_type { get; set; }
    }

    public class Invoicer
    {
        public Name name { get; set; }
        public Address address { get; set; }
        public Phone[] phones { get; set; }
        public string website { get; set; }
        public string tax_id { get; set; }
        public string logo_url { get; set; }
        public string additional_notes { get; set; }
    }

    public class Name
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public class Address
    {
        public string address_line_1 { get; set; }
        public string admin_area_2 { get; set; }
        public string admin_area_1 { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public class Phone
    {
        public string country_code { get; set; }
        public string national_number { get; set; }
        public string phone_type { get; set; }
    }

    public class Configuration
    {
        public Partial_Payment partial_payment { get; set; }
        public bool allow_tip { get; set; }
        public bool tax_calculated_after_discount { get; set; }
        public bool tax_inclusive { get; set; }
    }

    public class Partial_Payment
    {
        public bool allow_partial_payment { get; set; }
        public Minimum_Amount_Due minimum_amount_due { get; set; }
    }

    public class Minimum_Amount_Due
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Amount
    {
        public Breakdown breakdown { get; set; }
    }

    public class Breakdown
    {
        public Custom custom { get; set; }
        public Shipping shipping { get; set; }
        public Discount discount { get; set; }
    }

    public class Custom
    {
        public string label { get; set; }
        public Amount1 amount { get; set; }
    }

    public class Amount1
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Shipping
    {
        public Amount2 amount { get; set; }
        public Tax tax { get; set; }
    }

    public class Amount2
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public string name { get; set; }
        public string percent { get; set; }
    }

    public class Discount
    {
        public Invoice_Discount invoice_discount { get; set; }
    }

    public class Invoice_Discount
    {
        public string percent { get; set; }
    }

    public class Primary_Recipients
    {
        public Billing_Info billing_info { get; set; }
        public Shipping_Info shipping_info { get; set; }
    }

    public class Billing_Info
    {
        public Name1 name { get; set; }
        public Address1 address { get; set; }
        public string email_address { get; set; }
        public Phone1[] phones { get; set; }
        public string additional_info_value { get; set; }
    }

    public class Name1
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public class Address1
    {
        public string address_line_1 { get; set; }
        public string admin_area_2 { get; set; }
        public string admin_area_1 { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public class Phone1
    {
        public string country_code { get; set; }
        public string national_number { get; set; }
        public string phone_type { get; set; }
    }

    public class Shipping_Info
    {
        public Name2 name { get; set; }
        public Address2 address { get; set; }
    }

    public class Name2
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public class Address2
    {
        public string address_line_1 { get; set; }
        public string admin_area_2 { get; set; }
        public string admin_area_1 { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }
        public Unit_Amount unit_amount { get; set; }
        public Tax1 tax { get; set; }
        public Discount1 discount { get; set; }
        public string unit_of_measure { get; set; }
    }

    public class Unit_Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Tax1
    {
        public string name { get; set; }
        public string percent { get; set; }
    }

    public class Discount1
    {
        public string percent { get; set; }
        public Amount3 amount { get; set; }
    }

    public class Amount3
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
}
