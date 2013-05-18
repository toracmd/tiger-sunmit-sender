using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Data.OleDb;

namespace RenewEDSenderM.DbManager
{
    public class AccessData
    {
        private static string dbsource = "D:\\workspace\\sunmit\\recv\\20130428uScada1\\Database\\hisdb.mdb;";
        //public static string CONN_STRING1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + CommonClass.GetXmlNodeValue("DataConnectConfig.xml", "datasource") + ";User Id=" + CommonClass.GetXmlNodeValue("DataConnectConfig.xml", "userid") + ";Password=" + CommonClass.GetXmlNodeValue("DataConnectConfig.xml", "password") + ";";
        private static string CONN_STRING1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbsource;
        #region ExecuteNonQuery
        /// <summary>
        /// 执行SQL Query
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                int val = cmd.ExecuteNonQuery();
                conn.Close();////////////////////
                return val;

            }

        }
        /// <summary>
        /// 执行Query
        /// </summary>
        /// <param name="cmdText">SQL 语句</param>
        /// <param name="pa">参数列表</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string cmdText, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                if (pa != null)
                {
                    cmd.Parameters.AddRange(pa);
                }
                int val = cmd.ExecuteNonQuery();
                conn.Close();////////////////////
                return val;

            }

        }
        /// <summary>
        /// 执行Query
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <param name="cmdType">如何解释命令字符串</param>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="cmdParms">参数列表</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();////////////////////
                return val;
            }
        }
        /// <summary>
        /// 执行Query
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">如何解释命令字符串</param>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="cmdParms">参数列表</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
#endregion
        #region ExecuteReader
        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static OleDbDataReader ExecuteReader(string cmdText)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            OleDbConnection conn = new OleDbConnection(CONN_STRING1);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            try
            {
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        public static OleDbDataReader ExecuteReader(string cmdText, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            OleDbConnection conn = new OleDbConnection(CONN_STRING1);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            if (pa != null)
            {
                cmd.Parameters.AddRange(pa);
            }
            try
            {
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static OleDbDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            OleDbConnection conn = new OleDbConnection(connString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        #endregion
        #region ExecuteDataset
        /// <summary>
        /// (返回DataSet,不用参数)
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string cmdText)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                //create the DataAdapter &amp; DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();

                //fill the DataSet using default values for DataTable names, etc.
                da.Fill(ds);
                conn.Close();////////////////////
                //return the dataset
                return ds;
            }
        }
        public static DataSet ExecuteDataset(string cmdText, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                if (pa != null)
                {
                    cmd.Parameters.AddRange(pa);
                }
                //create the DataAdapter &amp; DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();

                //fill the DataSet using default values for DataTable names, etc.
                da.Fill(ds);
                conn.Close();////////////////////
                //return the dataset
                return ds;
            }
        }
        #endregion
        #region ExecuteDataRow
        /// <summary>
        ///
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static DataRow ExecuteDataRow(string cmdText)
        {
            OleDbCommand cmd = new OleDbCommand(); cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                //create the DataAdapter &amp; DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet myDS = new DataSet();

                da.Fill(myDS);
                conn.Close();////////////////////
                if (myDS.Tables[0].Rows.Count != 0)
                    return myDS.Tables[0].Rows[0];
                else
                    return null;
            }
        }
        public static DataRow ExecuteDataRow(string cmdText, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand(); cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                if (pa != null)
                {
                    cmd.Parameters.AddRange(pa);
                }
                //create the DataAdapter &amp; DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet myDS = new DataSet();

                da.Fill(myDS);
                conn.Close();////////////////////
                if (myDS.Tables[0].Rows.Count != 0)
                    return myDS.Tables[0].Rows[0];
                else
                    return null;
            }
        }
        #endregion
        #region ExecuteScalar
        /// <summary>
        ///
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;

                object val = cmd.ExecuteScalar();
                conn.Close();////////////////////
                return val;
            }
        }
        public static object ExecuteScalar(string cmdText, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                if (pa != null)
                {
                    cmd.Parameters.AddRange(pa);
                }
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                conn.Close();////////////////////
                return val;
            }
        }

        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = (int)cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                conn.Close();////////////////////
                return val;
            }
        }
        #endregion
        #region PrepareCommand
        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion
        #region ExecuteDataTable
        ///<summary>
        ///
        ///</summary>
        public static DataTable ExecuteDataTable(string cmdText)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                conn.Close();////////////////////
                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0];
                else
                    return null;
            }
        }
        public static DataTable ExecuteDataTable(string cmdText, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                if (pa != null)
                {
                    cmd.Parameters.AddRange(pa);
                }
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                conn.Close();////////////////////
                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0];
                else
                    return null;
            }
        }
        /// <summary>
        /// 按页返回固定行记录。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="CurrentPage">当前页</param>
        /// <param name="PageSize">页大小</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTableInFixedReords(string cmdText, int CurrentPage, int PageSize)
        {
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                OleDbDataAdapter sdr = new OleDbDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                int startIndex = PageSize * CurrentPage;
                sdr.Fill(ds, startIndex, PageSize, "tableName");
                conn.Close();////////////////////
                return ds.Tables["tableName"];
            }
        }
        /// <summary>
        /// 按页返回固定行记录。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="CurrentPage">当前页</param>
        /// <param name="PageSize">页大小</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTableInFixedReords(string cmdText, int CurrentPage, int PageSize, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(CONN_STRING1))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                if (pa != null)
                {
                    cmd.Parameters.AddRange(pa);
                }
                //create the DataAdapter &amp; DataSet
                OleDbDataAdapter sdr = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                int startIndex = PageSize * CurrentPage;
                sdr.Fill(ds, startIndex, PageSize, "tableName");
                conn.Close();////////////////////
                return ds.Tables["tableName"];
            }
        }
        #endregion
    }
    public class TestAccessData
    {
        public static void Test()
        {
            string cmdtext = "select * from DAY20130428";
            AccessData.ExecuteNonQuery(cmdtext);
            AccessData.ExecuteDataRow(cmdtext);
            AccessData.ExecuteDataset(cmdtext);
            AccessData.ExecuteDataTable(cmdtext);
            AccessData.ExecuteReader(cmdtext);
            AccessData.ExecuteScalar(cmdtext);
        }
    }
}
