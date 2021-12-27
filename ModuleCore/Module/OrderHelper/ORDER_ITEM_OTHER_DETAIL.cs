using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCore.Module.OrderHelper
{
    public class ORDER_ITEM_OTHER_DETAIL
    {
        public int ID { get; set; }
        public int ORDER_ITEMID { get; set; }
        public string ResponsibleParty { get; set; }
        public string Model { get; set; }
        public Nullable<bool> IsTransparency { get; set; }
        public string book_id { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public virtual ORDER_ITEM_DETAIL ORDER_ITEM_DETAIL { get; set; }
    }
}
