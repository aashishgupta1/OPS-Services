using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCore.Module.OrderHelper
{
    public class Order_Other_Detail
    {
        public int ID { get; set; }
        public int Order_ID { get; set; }
        public string Order_ORDERID { get; set; }
        public Nullable<System.DateTime> LatestShipDate { get; set; }
        public Nullable<bool> IsReplacementOrder { get; set; }
        public Nullable<int> NumberOfItemsShipped { get; set; }
        public string ShipServiceLevel { get; set; }
        public string ShippedByAmazonTFM { get; set; }
        public Nullable<short> IsBusinessOrder { get; set; }
        public Nullable<short> NumberOfItemsUnshipped { get; set; }
        public Nullable<System.DateTime> LatestDeliveryDate { get; set; }
        public Nullable<bool> IsGlobalExpressEnabled { get; set; }
        public Nullable<bool> IsSoldByAB { get; set; }
        public Nullable<System.DateTime> EarliestDeliveryDate { get; set; }
        public Nullable<bool> IsPremiumOrder { get; set; }
        public Nullable<System.DateTime> EarliestShipDate { get; set; }
        public string MarketplaceId { get; set; }
        public string FulfillmentChannel { get; set; }
        public string PaymentMethod { get; set; }
        public Nullable<bool> IsISPU { get; set; }
        public Nullable<bool> IsPrime { get; set; }
        public string ShipmentServiceLevelCategory { get; set; }
        public string RequestId { get; set; }
        public string buyer_id { get; set; }
        public string buyerPurchaseOrder_id { get; set; }
        public string reseller_id { get; set; }
        public string seller_id { get; set; }
        public string maxDeliveryDays { get; set; }
        public string minDeliveryDays { get; set; }
        public string payments_transaction_id { get; set; }
        public string batch_id { get; set; }
        public string Customer_PO { get; set; }
        public string Alibris_ID_Number { get; set; }
        public string Link { get; set; }
        public string Link_to_coupon { get; set; }
        public string Rental { get; set; }
        public Nullable<decimal> Rent_Price { get; set; }
        public Nullable<System.DateTime> Cus_Return_Date { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Order_Source { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public virtual Order_Detail Order_Detail { get; set; }
    }
}
