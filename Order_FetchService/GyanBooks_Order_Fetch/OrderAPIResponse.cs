namespace GyanBooks_Order_Fetch
{
    public class OrderAPIResponse
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string ordercode { get; set; }
        public string orderno { get; set; }
        public string orderdate { get; set; }
        public string ipaddress { get; set; }
        public string ordemailid { get; set; }
        public string orddiscount { get; set; }
        public string ordprefix { get; set; }
        public string ordname { get; set; }
        public string ordorganisation { get; set; }
        public string orddepartment { get; set; }
        public string ordaddress { get; set; }
        public string ordcity { get; set; }
        public string ordstate { get; set; }
        public string ordzip { get; set; }
        public string ordcountry { get; set; }
        public string ordphone { get; set; }
        public string ordmobile { get; set; }
        public string paymentmode { get; set; }
        public string remarks { get; set; }
        public string ordcurr { get; set; }
        public string ordamt { get; set; }
        public string paymentstatus { get; set; }
        public string paymentremarks { get; set; }
        public string orderstatus { get; set; }
        public string orderremarks { get; set; }
        public string shippingstatus { get; set; }
        public string shippingremarks { get; set; }
        public string couponcode { get; set; }
        public string coupondisc { get; set; }
        public string toshow { get; set; }
        public string ordlogs { get; set; }
        public string lastupdate { get; set; }
        public string orderid { get; set; }
        public string bookcode { get; set; }
        public string binding { get; set; }
        public string bookdesc { get; set; }
        public string currency { get; set; }
        public string rate { get; set; }
        public string qty { get; set; }
        public string amount { get; set; }
        public string status_ordersdet { get; set; }
        public string lastupdate_ordersdet { get; set; }
        public string orderid_ordersdet { get; set; }
    }

}
