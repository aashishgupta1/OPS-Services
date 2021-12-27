using System;

namespace Bonanza_Order_Fetch.Model
{


    public class OrderResponse
    {
        public string ack { get; set; }
        public string version { get; set; }
        public DateTime timestamp { get; set; }
        public Getordersresponse getOrdersResponse { get; set; }
    }

    public class Getordersresponse
    {
        public Orderarray[] orderArray { get; set; }
        public string hasMoreOrders { get; set; }
        public Paginationresult paginationResult { get; set; }
        public int pageNumber { get; set; }
    }

    public class Paginationresult
    {
        public int totalNumberOfEntries { get; set; }
        public int totalNumberOfPages { get; set; }
    }

    public class Orderarray
    {
        public Order order { get; set; }
    }

    public class Order
    {
        public string amountPaid { get; set; }
        public float amountSaved { get; set; }
        public string buyerCheckoutMessage { get; set; }
        public int buyerUserID { get; set; }
        public string buyerUserName { get; set; }
        public Checkoutstatus checkoutStatus { get; set; }
        public DateTime createdTime { get; set; }
        public string creatingUserRole { get; set; }
        public string currencyCode { get; set; }
        public Itemarray[] itemArray { get; set; }
        public int orderID { get; set; }
        public string orderStatus { get; set; }
        public float subtotal { get; set; }
        public float taxAmount { get; set; }
        public object telephoneNumber { get; set; }
        public string total { get; set; }
        public Transactionarray transactionArray { get; set; }
        public DateTime paidTime { get; set; }
        public DateTime shippedTime { get; set; }
        public Shippingaddress shippingAddress { get; set; }
        public Shippingdetails shippingDetails { get; set; }
    }

    public class Checkoutstatus
    {
        public string status { get; set; }
    }

    public class Transactionarray
    {
        public Transaction transaction { get; set; }
    }

    public class Transaction
    {
        public Buyer buyer { get; set; }
        public string providerName { get; set; }
        public string providerID { get; set; }
        public string finalValueFee { get; set; }
    }

    public class Buyer
    {
        public string email { get; set; }
    }

    public class Shippingaddress
    {
        public int addressID { get; set; }
        public string cityName { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string name { get; set; }
        public string postalCode { get; set; }
        public string stateOrProvince { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
    }

    public class Shippingdetails
    {
        public int insuranceFee { get; set; }
        public string amount { get; set; }
        public object[] servicesArray { get; set; }
        public string shippingService { get; set; }
        public dynamic shipmentTrackingNumber { get; set; }
        public string notes { get; set; }
    }

    public class Shipmenttrackingnumber
    {
       
    }

    public class Itemarray
    {
        public Item item { get; set; }
    }

    public class Item
    {
        public int itemID { get; set; }
        public int parentItemID { get; set; }
        public string sellerInventoryID { get; set; }
        public string sku { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string thumbnailURL { get; set; }
        public int quantity { get; set; }
        public object[] personalizedText { get; set; }
    }

}
