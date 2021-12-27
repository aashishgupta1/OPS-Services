using DBInteraction;
using DBProject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ModuleCore.Module.OrderHelper
{
    public static class OrderHelper
    {
        private static OPSEntities1 contextEntity = new OPSEntities1();
        private static DataRepository<Order_Detail> dataRepository = new DataRepository<Order_Detail>(contextEntity);
        
        public static List<Order_Detail> GetOrderDetailsByOrderID(string sOrderID)
        {
            var orderData = dataRepository.GetAll().Include(y => y.Order_Other_Detail).Include(x => x.ORDER_ITEM_DETAIL).Where(cond => cond.Order_id == sOrderID).ToList();
            return orderData;
            //DataTable dt = new DataTable();
            //DBOperation.sConnectionString = sConnectionString;
            //DBOperation.StructDBOperation[] structDBOperation = new DBOperation.StructDBOperation[1];
            //int iParam = 0;
            //structDBOperation[iParam] = new DBOperation.StructDBOperation();
            //structDBOperation[iParam].sParamName = "@OrderID";
            //structDBOperation[iParam].sParamType = System.Data.SqlDbType.VarChar;
            //structDBOperation[iParam].sParamValue = sOrderID;

            //string sRetVal =  DBOperation.ExecuteDBOperation("sp_GetOrderDetails", DBOperation.OperationType.STOREDPROC, structDBOperation, ref dt);
            //if(sRetVal == "SUCCESS")
            //{
            //    List<Order_Detail> lstorder_Detail = new List<Order_Detail>();

            //    foreach(DataRow dataRow in dt.Rows)
            //    {
            //        Order_Detail order_Detail = new Order_Detail();
            //        order_Detail.ID = Convert.ToInt32( dataRow["id"]);
            //    }
            //}
            //else
            //{
            //}
        }
        public static Order_Detail InsertOrderInfo(Order_Detail order)
        {
            var orderData = dataRepository.Create(order);
            return orderData;
        }
        public static List<Order_Detail> GetOrderInfo(Func<Order_Detail, bool> predicate)
        {
            var orderData = dataRepository.GetByFilter(predicate).ToList();
            return orderData;
        }
    }
}
