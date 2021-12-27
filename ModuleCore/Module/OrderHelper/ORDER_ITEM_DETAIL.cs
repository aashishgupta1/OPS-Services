using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCore.Module.OrderHelper
{
    public class ORDER_ITEM_DETAIL
    {
        public int ID { get; set; }
        public int Order_ID { get; set; }
        public string Order_ORDERID { get; set; }
        public string OrderItemId { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public Nullable<long> isbn { get; set; }
        public string ItemCode { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public Nullable<decimal> ShippingTax { get; set; }
        public Nullable<decimal> PromotionDiscount { get; set; }
        public Nullable<decimal> ShippingDiscountTax { get; set; }
        public Nullable<decimal> ShippingPrice { get; set; }
        public Nullable<decimal> ItemPrice { get; set; }
        public Nullable<decimal> ItemTax { get; set; }
        public Nullable<decimal> ShippingDiscount { get; set; }
        public Nullable<decimal> PromotionDiscountTax { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string CurrencyCode { get; set; }
        public Nullable<int> NumberOfItems { get; set; }
        public Nullable<int> QuantityShipped { get; set; }
        public string ConditionId { get; set; }
        public Nullable<bool> IsGift { get; set; }
        public string ASIN { get; set; }
        public string ConditionSubtypeId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public virtual ICollection<ORDER_ITEM_OTHER_DETAIL> ORDER_ITEM_OTHER_DETAIL { get; set; }
    }
}
