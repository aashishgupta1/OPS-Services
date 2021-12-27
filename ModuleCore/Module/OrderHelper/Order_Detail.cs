using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCore.Module.OrderHelper
{
    public class Order_Detail
    {
        public int ID { get; set; }
        public string Order_id { get; set; }
        public string OrderType { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public string OrderStatus { get; set; }
        public string SalesChannel { get; set; }
        public string payment_Status { get; set; }
        public Nullable<System.DateTime> Payment_Date { get; set; }
        public string PaymentMethodDetail { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public Nullable<decimal> gst { get; set; }
        public Nullable<decimal> handling { get; set; }
        public Nullable<decimal> shipping { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string trackingCode { get; set; }
        public Nullable<int> Order_Quantity { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Municipality { get; set; }
        public Nullable<bool> isAddressSharingConfidential { get; set; }
        public string StateOrRegion { get; set; }
        public string Phone { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Country { get; set; }
        public string Ship_Method { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        public virtual ICollection<ORDER_ITEM_DETAIL> ORDER_ITEM_DETAIL { get; set; }
        public virtual ICollection<Order_Other_Detail> Order_Other_Detail { get; set; }
    }
}
