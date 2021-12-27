
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class orderUpdateResponse1
{

    private orderUpdateResponsePurchaseOrder[] purchaseOrderListField;

    private decimal versionField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("purchaseOrder", IsNullable = false)]
    public orderUpdateResponsePurchaseOrder[] purchaseOrderList
    {
        get
        {
            return this.purchaseOrderListField;
        }
        set
        {
            this.purchaseOrderListField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal version
    {
        get
        {
            return this.versionField;
        }
        set
        {
            this.versionField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrder
{

    private orderUpdateResponsePurchaseOrderBuyer buyerField;

    private orderUpdateResponsePurchaseOrderBuyerPurchaseOrder buyerPurchaseOrderField;

    private orderUpdateResponsePurchaseOrderDomain domainField;

    private orderUpdateResponsePurchaseOrderOrderDate orderDateField;

    private orderUpdateResponsePurchaseOrderOrderTotals orderTotalsField;

    private string purchaseMethodField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemList purchaseOrderItemListField;

    private orderUpdateResponsePurchaseOrderReseller resellerField;

    private orderUpdateResponsePurchaseOrderSeller sellerField;

    private orderUpdateResponsePurchaseOrderShipping shippingField;

    private object specialInstructionsField;

    private orderUpdateResponsePurchaseOrderStatus statusField;

    private uint idField;

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderBuyer buyer
    {
        get
        {
            return this.buyerField;
        }
        set
        {
            this.buyerField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderBuyerPurchaseOrder buyerPurchaseOrder
    {
        get
        {
            return this.buyerPurchaseOrderField;
        }
        set
        {
            this.buyerPurchaseOrderField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderDomain domain
    {
        get
        {
            return this.domainField;
        }
        set
        {
            this.domainField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderDate orderDate
    {
        get
        {
            return this.orderDateField;
        }
        set
        {
            this.orderDateField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotals orderTotals
    {
        get
        {
            return this.orderTotalsField;
        }
        set
        {
            this.orderTotalsField = value;
        }
    }

    /// <remarks/>
    public string purchaseMethod
    {
        get
        {
            return this.purchaseMethodField;
        }
        set
        {
            this.purchaseMethodField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemList purchaseOrderItemList
    {
        get
        {
            return this.purchaseOrderItemListField;
        }
        set
        {
            this.purchaseOrderItemListField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderReseller reseller
    {
        get
        {
            return this.resellerField;
        }
        set
        {
            this.resellerField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderSeller seller
    {
        get
        {
            return this.sellerField;
        }
        set
        {
            this.sellerField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderShipping shipping
    {
        get
        {
            return this.shippingField;
        }
        set
        {
            this.shippingField = value;
        }
    }

    /// <remarks/>
    public object specialInstructions
    {
        get
        {
            return this.specialInstructionsField;
        }
        set
        {
            this.specialInstructionsField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderStatus status
    {
        get
        {
            return this.statusField;
        }
        set
        {
            this.statusField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderBuyer
{

    private string emailField;

    private orderUpdateResponsePurchaseOrderBuyerMailingAddress mailingAddressField;

    private uint idField;

    /// <remarks/>
    public string email
    {
        get
        {
            return this.emailField;
        }
        set
        {
            this.emailField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderBuyerMailingAddress mailingAddress
    {
        get
        {
            return this.mailingAddressField;
        }
        set
        {
            this.mailingAddressField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderBuyerMailingAddress
{

    private string cityField;

    private uint codeField;
    private string nameField;

    private string phoneField;

    private string regionField;

    private string streetField;

    private object street2Field;

    /// <remarks/>
    public string city
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public uint code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }

    /// <remarks/>
    public string country { get; set; }

    /// <remarks/>
    public string name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string phone
    {
        get
        {
            return this.phoneField;
        }
        set
        {
            this.phoneField = value;
        }
    }

    /// <remarks/>
    public string region
    {
        get
        {
            return this.regionField;
        }
        set
        {
            this.regionField = value;
        }
    }

    /// <remarks/>
    public string street
    {
        get
        {
            return this.streetField;
        }
        set
        {
            this.streetField = value;
        }
    }

    /// <remarks/>
    public object street2
    {
        get
        {
            return this.street2Field;
        }
        set
        {
            this.street2Field = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderBuyerPurchaseOrder
{

    private uint idField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderDomain
{

    private string nameField;

    private byte idField;

    /// <remarks/>
    public string name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderDate
{

    private orderUpdateResponsePurchaseOrderOrderDateDate dateField;

    private orderUpdateResponsePurchaseOrderOrderDateTime timeField;

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderDateDate date
    {
        get
        {
            return this.dateField;
        }
        set
        {
            this.dateField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderDateTime time
    {
        get
        {
            return this.timeField;
        }
        set
        {
            this.timeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderDateDate
{

    private byte dayField;

    private byte monthField;

    private ushort yearField;

    /// <remarks/>
    public byte day
    {
        get
        {
            return this.dayField;
        }
        set
        {
            this.dayField = value;
        }
    }

    /// <remarks/>
    public byte month
    {
        get
        {
            return this.monthField;
        }
        set
        {
            this.monthField = value;
        }
    }

    /// <remarks/>
    public ushort year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderDateTime
{

    private byte hourField;

    private byte minuteField;

    private byte secondField;

    /// <remarks/>
    public byte hour
    {
        get
        {
            return this.hourField;
        }
        set
        {
            this.hourField = value;
        }
    }

    /// <remarks/>
    public byte minute
    {
        get
        {
            return this.minuteField;
        }
        set
        {
            this.minuteField = value;
        }
    }

    /// <remarks/>
    public byte second
    {
        get
        {
            return this.secondField;
        }
        set
        {
            this.secondField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotals
{

    private orderUpdateResponsePurchaseOrderOrderTotalsGst gstField;

    private orderUpdateResponsePurchaseOrderOrderTotalsHandling handlingField;

    private orderUpdateResponsePurchaseOrderOrderTotalsShipping shippingField;

    private orderUpdateResponsePurchaseOrderOrderTotalsSubtotal subtotalField;

    private orderUpdateResponsePurchaseOrderOrderTotalsTax taxField;

    private orderUpdateResponsePurchaseOrderOrderTotalsTotal totalField;

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotalsGst gst
    {
        get
        {
            return this.gstField;
        }
        set
        {
            this.gstField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotalsHandling handling
    {
        get
        {
            return this.handlingField;
        }
        set
        {
            this.handlingField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotalsShipping shipping
    {
        get
        {
            return this.shippingField;
        }
        set
        {
            this.shippingField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotalsSubtotal subtotal
    {
        get
        {
            return this.subtotalField;
        }
        set
        {
            this.subtotalField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotalsTax tax
    {
        get
        {
            return this.taxField;
        }
        set
        {
            this.taxField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderOrderTotalsTotal total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotalsGst
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotalsHandling
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotalsShipping
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotalsSubtotal
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotalsTax
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderOrderTotalsTotal
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemList
{

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItem purchaseOrderItemField;

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItem purchaseOrderItem
    {
        get
        {
            return this.purchaseOrderItemField;
        }
        set
        {
            this.purchaseOrderItemField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItem
{

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemBook bookField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDate orderDateField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemPurchaseOrder purchaseOrderField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemSellerTotal sellerTotalField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemStatus statusField;

    private uint idField;

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemBook book
    {
        get
        {
            return this.bookField;
        }
        set
        {
            this.bookField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDate orderDate
    {
        get
        {
            return this.orderDateField;
        }
        set
        {
            this.orderDateField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemPurchaseOrder purchaseOrder
    {
        get
        {
            return this.purchaseOrderField;
        }
        set
        {
            this.purchaseOrderField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemSellerTotal sellerTotal
    {
        get
        {
            return this.sellerTotalField;
        }
        set
        {
            this.sellerTotalField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemStatus status
    {
        get
        {
            return this.statusField;
        }
        set
        {
            this.statusField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemBook
{

    private string authorField;

    private string descriptionField;

    private object isbnField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemBookPrice priceField;

    private string titleField;

    private string vendorKeyField;

    private ulong idField;

    /// <remarks/>
    public string author
    {
        get
        {
            return this.authorField;
        }
        set
        {
            this.authorField = value;
        }
    }

    /// <remarks/>
    public string description
    {
        get
        {
            return this.descriptionField;
        }
        set
        {
            this.descriptionField = value;
        }
    }

    /// <remarks/>
    public object isbn
    {
        get
        {
            return this.isbnField;
        }
        set
        {
            this.isbnField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemBookPrice price
    {
        get
        {
            return this.priceField;
        }
        set
        {
            this.priceField = value;
        }
    }

    /// <remarks/>
    public string title
    {
        get
        {
            return this.titleField;
        }
        set
        {
            this.titleField = value;
        }
    }

    /// <remarks/>
    public string vendorKey
    {
        get
        {
            return this.vendorKeyField;
        }
        set
        {
            this.vendorKeyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ulong id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemBookPrice
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDate
{

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDateDate dateField;

    private orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDateTime timeField;

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDateDate date
    {
        get
        {
            return this.dateField;
        }
        set
        {
            this.dateField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDateTime time
    {
        get
        {
            return this.timeField;
        }
        set
        {
            this.timeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDateDate
{

    private byte dayField;

    private byte monthField;

    private ushort yearField;

    /// <remarks/>
    public byte day
    {
        get
        {
            return this.dayField;
        }
        set
        {
            this.dayField = value;
        }
    }

    /// <remarks/>
    public byte month
    {
        get
        {
            return this.monthField;
        }
        set
        {
            this.monthField = value;
        }
    }

    /// <remarks/>
    public ushort year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemOrderDateTime
{

    private byte hourField;

    private byte minuteField;

    private byte secondField;

    /// <remarks/>
    public byte hour
    {
        get
        {
            return this.hourField;
        }
        set
        {
            this.hourField = value;
        }
    }

    /// <remarks/>
    public byte minute
    {
        get
        {
            return this.minuteField;
        }
        set
        {
            this.minuteField = value;
        }
    }

    /// <remarks/>
    public byte second
    {
        get
        {
            return this.secondField;
        }
        set
        {
            this.secondField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemPurchaseOrder
{

    private uint idField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemSellerTotal
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderPurchaseOrderItemListPurchaseOrderItemStatus
{

    private byte codeField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderReseller
{

    private string nameField;

    private uint idField;

    /// <remarks/>
    public string name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderSeller
{

    private uint idField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderShipping
{

    private object companyField;

    private orderUpdateResponsePurchaseOrderShippingExtraItemShippingCost extraItemShippingCostField;

    private orderUpdateResponsePurchaseOrderShippingFirstItemShippingCost firstItemShippingCostField;

    private byte maxDeliveryDaysField;

    private byte minDeliveryDaysField;

    private object trackingCodeField;

    /// <remarks/>
    public object company
    {
        get
        {
            return this.companyField;
        }
        set
        {
            this.companyField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderShippingExtraItemShippingCost extraItemShippingCost
    {
        get
        {
            return this.extraItemShippingCostField;
        }
        set
        {
            this.extraItemShippingCostField = value;
        }
    }

    /// <remarks/>
    public orderUpdateResponsePurchaseOrderShippingFirstItemShippingCost firstItemShippingCost
    {
        get
        {
            return this.firstItemShippingCostField;
        }
        set
        {
            this.firstItemShippingCostField = value;
        }
    }

    /// <remarks/>
    public byte maxDeliveryDays
    {
        get
        {
            return this.maxDeliveryDaysField;
        }
        set
        {
            this.maxDeliveryDaysField = value;
        }
    }

    /// <remarks/>
    public byte minDeliveryDays
    {
        get
        {
            return this.minDeliveryDaysField;
        }
        set
        {
            this.minDeliveryDaysField = value;
        }
    }

    /// <remarks/>
    public object trackingCode
    {
        get
        {
            return this.trackingCodeField;
        }
        set
        {
            this.trackingCodeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderShippingExtraItemShippingCost
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderShippingFirstItemShippingCost
{

    private string currencyField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public decimal Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class orderUpdateResponsePurchaseOrderStatus
{

    private byte codeField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

