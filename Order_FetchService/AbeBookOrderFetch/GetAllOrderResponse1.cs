using System.Collections.Generic;

namespace AbeBookOrderFetch
{
    public class orderUpdateResponse
    {
        public purchaseOrderList purchaseOrderList { get; set; }
    }
    public class purchaseOrderList
    {
        public List<purchaseOrder> purchaseOrder { get; set; }
    }
    public class purchaseOrder
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

        public buyer buyer { get; set; }
        public buyerPurchaseOrder buyerPurchaseOrder { get; set; }
        public domain domain { get; set; }
        public orderDate orderDate { get; set; }
        public orderTotals orderTotals { get; set; }
        public string purchaseMethod { get; set; }
        public purchaseOrderItemList purchaseOrderItemList { get; set; }
        public reseller reseller { get; set; }
        public seller seller { get; set; }
        public purchaseOrdershipping shipping { get; set; }
        public string specialInstructions { get; set; }
        public status status { get; set; }
    }
    public class buyer
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

        public string email { get; set; }
        public mailingAddress mailingAddress { get; set; }
    }
    public class mailingAddress
    {
        public string city { get; set; }
        public string code { get; set; }
        public string country { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string region { get; set; }
        public string street { get; set; }
        public string street2 { get; set; }

    }
    public class buyerPurchaseOrder
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }
    }
    public class domain
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }
        public string name { get; set; }
    }
    public class orderDate
    {
        public date date { get; set; }
        public time time { get; set; }
    }
    public class date
    {
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
    }
    public class time
    {
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
    }

    public class orderTotals
    {
        public gst gst { get; set; }
        public handling handling { get; set; }
        public shipping shipping { get; set; }
        public subtotal subtotal { get; set; }
        public tax tax { get; set; }
        public total total { get; set; }

    }
    public class gst
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }

    public class handling
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }
    public class shipping
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }
    public class subtotal
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }
    public class tax
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }
    public class total
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }

    public class purchaseOrderItemList
    {
        public List<purchaseOrderItem> purchaseOrderItem { get; set; }
    }

    public class purchaseOrderItem
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }
        public book book { get; set; }
        public orderDate orderDate { get; set; }
        public sellerTotal sellerTotal { get; set; }
        public status status { get; set; }
    }
    public class book
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string isbn { get; set; }
        public string title { get; set; }
        public string vendorKey { get; set; }
        public price price { get; set; }

    }
    public class price
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }

    }
    public class sellerTotal
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }

    }
    public class status
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string code { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }
    }
    public class reseller
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

        public string name { get; set; }
    }
    public class seller
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

    }
    public class purchaseOrdershipping
    {
        public string company { get; set; }
        public extraItemShippingCost extraItemShippingCost { get; set; }
        public firstItemShippingCost firstItemShippingCost { get; set; }
        public string maxDeliveryDays { get; set; }
        public string minDeliveryDays { get; set; }
        public string trackingCode { get; set; }
    }
    public class extraItemShippingCost
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }

    }
    public class firstItemShippingCost
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string value { get; set; }

    }
}
