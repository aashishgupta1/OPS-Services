using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using ModuleCore;
using System;
using System.Data;
using System.Data.SqlClient;

namespace PaymentLinkOrderFilteration
{
    public class FilterOrdersForPayment : ModuleBase
    {
        string connectionString = "Server={0};Database={1};User Id={2};Password={3};";
        public FilterOrdersForPayment(ILogger logger, ISettings settings) : base(logger, settings)
        {
        }

        public override void Run()
        {
            DataTable table = new DataTable();
            table = GetOrderFromDB();
            InsertIntoPaymentLink(table);
        }
        private DataTable GetOrderFromDB()
        {
            DataTable dt = new DataTable();
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT od.ID, od.OrderId, od.email, od.OrderItemID, od.bookID, od.orderTotals_total_amount as price, od.isbn FROM OrderDetails od LEFT JOIN PAYMENTLINK pl ON od.OrderID = pl.OrderID WHERE pl.ID IS NULL AND od.purchaseMethod='SD'", connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    dt.Load(dataReader);
                }
            }
            return dt;
        }
        private void InsertIntoPaymentLink(DataTable dt)
        {
            connectionString = string.Format(connectionString, ModuleSetings.ODBCServerName, ModuleSetings.ODBCDatabase, ModuleSetings.ODBCUserName, ModuleSetings.ODBCPassword);
            DataTable table = new DataTable();
            DBOperation.sConnectionString = connectionString;
            DBOperation.StructDBOperation[] structDBOperations = new DBOperation.StructDBOperation[8];
            for (int i = 0; i < 7; i++)
            {
                structDBOperations[i] = new DBOperation.StructDBOperation();
            }

            foreach (DataRow drow in dt.Rows)
            {
                int iParam = 0;
                structDBOperations[iParam].sParamName = "@Order_ID";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["ID"]);
                structDBOperations[iParam].sParamType = SqlDbType.Int;

                iParam++;
                structDBOperations[iParam].sParamName = "@OrderID";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["OrderID"]);
                structDBOperations[iParam].sParamType = SqlDbType.VarChar;

                iParam++;
                structDBOperations[iParam].sParamName = "@email";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["email"]);
                structDBOperations[iParam].sParamType = SqlDbType.VarChar;

                iParam++;
                structDBOperations[iParam].sParamName = "@OrderItemID";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["OrderItemID"]);
                structDBOperations[iParam].sParamType = SqlDbType.VarChar;

                iParam++;
                structDBOperations[iParam].sParamName = "@bookID";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["bookID"]);
                structDBOperations[iParam].sParamType = SqlDbType.VarChar;

                iParam++;
                structDBOperations[iParam].sParamName = "@isbn";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["isbn"]);
                structDBOperations[iParam].sParamType = SqlDbType.Int;

                iParam++;
                structDBOperations[iParam].sParamName = "@price";
                structDBOperations[iParam].sParamValue = Convert.ToString(drow["price"]);
                structDBOperations[iParam].sParamType = SqlDbType.BigInt;

                iParam++;
                structDBOperations[iParam].sParamName = "@paymentGatewayName";
                structDBOperations[iParam].sParamValue = Convert.ToString(PaymentGateway.PAYPAL);
                structDBOperations[iParam].sParamType = SqlDbType.BigInt;

                string sRetVal = DBOperation.ExecuteDBOperation("InsertPaymentLinkGeneration", DBOperation.OperationType.STOREDPROC, structDBOperations, ref table);
                if (sRetVal != "SUCCESS")
                {
                    logger.LogError(sRetVal);
                }
            }
        }
    }
}
