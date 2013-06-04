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
        /// <summary>
        /// 采集数据库路径
        /// </summary>
        //private static string dbsource1 = "../../../../RenewEDSenderM/database/hisdb.mdb";
        private static readonly string dbsource1 = System.Configuration.ConfigurationManager.AppSettings["CONN_HISDB"];
        /// <summary>
        /// 上传数据库路径
        /// </summary>
        //private static string dbsource2 = "../../../../RenewEDSenderM/database/info.mdb";
        private static readonly string dbsource2 = System.Configuration.ConfigurationManager.AppSettings["CONN_INFO"];
        // for 单体测试
        //public static string CONN_STRING1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbsource1 + "D:/workspace/sunmit/svn/RenewEDSenderM/database/hisdb.mdb;";
        //public static string CONN_STRING2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbsource2 + "D:/workspace/sunmit/svn/RenewEDSenderM/database/info.mdb;";
        public static string CONN_STRING1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbsource1;
        public static string CONN_STRING2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbsource2;
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
        public static int ExecuteNonQuery(string cmdText, string connString, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(connString))
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
        public static int ExecuteNonQueryScalar(string cmdText, string connString, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            string query = "Select @@Identity";
            using (OleDbConnection conn = new OleDbConnection(connString))
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
                cmd.CommandText = query;
                int ID = (int)cmd.ExecuteScalar();
                conn.Close();////////////////////
                return ID;
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
        public static DataRow ExecuteDataRow(string cmdText, string connString, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand(); cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(connString))
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
        public static DataTable ExecuteDataTable(string cmdText, string connString)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(connString))
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
        public static DataTable ExecuteDataTable(string cmdText,  string connString, params OleDbParameter[] pa)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = 120;
            using (OleDbConnection conn = new OleDbConnection(connString))
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
            int i = 1;
            switch(i)
            {
                case 1:
                //测试计算平均值，并存入采集数据库
                    DateTime date_send = new DateTime(2013, 4, 28, 19, 0, 0);   //2013-04-28 19:00:00
                    TimeSpan ts = new TimeSpan(2, 0, 0);    //2小时的间隔
                    History_Data hd;
                    Single [] dr = DataDump.CalculateAverage(date_send, ts);
                    DataDump.WriteToHisDb(date_send, dr, out hd);
                    DataDump.update_Upload(hd.id);
                    break;
                //更新发送后的数据
                case 3:
                //取得历史失败数据
                    DateTime begin_time = new DateTime(2013, 4, 28, 19, 0, 0);
                    DateTime end_time = new DateTime(2013, 4, 28, 19, 0, 0);

                    History_Data[] hd_array = DataDump.FetchDataFail(begin_time, end_time);
                    break;
                //取得历史成功数据
                case 4:
                    begin_time = new DateTime(2013, 4, 28, 19, 0, 0);
                    end_time = new DateTime(2013, 4, 28, 19, 0, 0);
                    History_Data[] hd_array_suc = DataDump.FetchDataSuccess(begin_time, end_time);
                    break;
            }
        }
    }
    /// <summary>
    /// 上传服务数据处理逻辑类
    /// </summary>
    public class DataDump
    {
        /// <summary>
        /// 计算4个指标在当前采样时间段内的平均值，并存储到发送历史数据库中
        /// </summary>
        /// <param name="FixedTime">定时发送时刻</param>
        /// <param name="FixedCycleT">定时发送周期</param>
        /// 
        public static /*DataRow[]*/ Single[] CalculateAverage(DateTime FixedTime, TimeSpan FixedCycleT)
        {
            //从采集数据库中提取区间[dt-nCycle,dt]内的监测值
            
            //当前时间的采集值所在的表
            string tbl_name_collect = "DAY" + FixedTime.Date.ToString("yyyyMMdd");
            
            //计算区间
            DateTime dt_end = FixedTime;
            DateTime dt_start = dt_end - FixedCycleT;

            //两天之间的问题
            if (dt_end.Date > dt_start.Date)
            {
                dt_start = dt_end.Date;
            }
            //取得dt的小时，分，秒
            int dt_end_hour = dt_end.Hour;
            int dt_end_min = dt_end.Minute;
            int dt_end_sec = dt_end.Second;
            //取得dt_start的小时，分，秒
            int dt_start_hour = dt_start.Hour;
            int dt_start_min = dt_start.Minute;
            int dt_start_sec = dt_start.Second;
            
            //获取此区间的监测指标值
            //SQL语句 计算平均值
            //T.B.D. 字段未确定
            string sql_hour = " VARIANTNAME=@VARIANTNAME and HOURF between @HOUR_L and @HOUR_H ";
            string sql_hour_low = " or VARIANTNAME=@VARIANTNAME and HOURF = @HOUR_L and MINUTEF between @MINUTE_L and 60 ";
            string sql_hour_high = " or VARIANTNAME=@VARIANTNAME and HOURF = @HOUR_H and MINUTEF between 0 and @MINUTE_L ";
            string sql_hour_minute_low = " or VARIANTNAME=@VARIANTNAME and HOURF = @HOUR_L and MINUTEF = @HOUR_L and SECONDF between @SECOND_L and 60 ";
            string sql_hour_minute_high = " or VARIANTNAME=@VARIANTNAME and HOURF = @HOUR_H and MINUTEF = @HOUR_H and SECONDF between 0 and @SECOND_H ";
            string sql = @"select avg(DATA) from " + tbl_name_collect + " where " + sql_hour + sql_hour_high + sql_hour_low + sql_hour_minute_high + sql_hour_minute_low;
                /*
                + " where VARIANTNAME=@VARIANTNAME and HOURF >= @HOUR_L and HOURF <= @HOUR_H "
                + " and MINUTEF >= @MINUTE_L and MINUTEF <= @MINUTE_H "
                + " and SECONDF >= @SECOND_L and SECONDF <= @SECONDF_H "
                + ";";
                 * */
            //SQL语句 计算发电量
            string sql2 = @"select Max(DATA)  from " + tbl_name_collect + " where " + sql_hour + sql_hour_high + sql_hour_low + sql_hour_minute_high + sql_hour_minute_low;
                /*
                + " where VARIANTNAME=@VARIANTNAME and HOURF >= @HOUR_L and HOURF <= @HOUR_H "
                + " and MINUTEF >= @MINUTE_L and MINUTEF <= @MINUTE_H "
                + " and SECONDF >= @SECOND_L and SECONDF <= @SECONDF_H "
                + " order by DATA desc ;";
                 * */
            //平行于光伏组件的太阳辐照度 总辐射 气象仪_3
            OleDbParameter[] parameters_1  = {
                new OleDbParameter("@VARIANTNAME",  "气象仪_3"),
                new OleDbParameter("@HOUR_L", dt_start_hour),
                new OleDbParameter("@HOUR_H", dt_end_hour),
                new OleDbParameter("@MINUTE_L", dt_start_min),
                new OleDbParameter("@MINUTE_H", dt_end_min),
                new OleDbParameter("@SECOND_L", dt_start_sec),
                new OleDbParameter("@SECOND_H", dt_end_sec)
            };
            //室外温度 大气温度 气象仪_8
            OleDbParameter[] parameters_2 = {
                new OleDbParameter("@VARIANTNAME",  "气象仪_8"),
                new OleDbParameter("@HOUR_L", dt_start_hour),
                new OleDbParameter("@HOUR_H", dt_end_hour),
                new OleDbParameter("@MINUTE_L", dt_start_min),
                new OleDbParameter("@MINUTE_H", dt_end_min),
                new OleDbParameter("@SECOND_L", dt_start_sec),
                new OleDbParameter("@SECOND_H", dt_end_sec)
            };
            //光伏组件背面表面温度 大地温度 气象仪_9
            OleDbParameter[] parameters_3 = {
                new OleDbParameter("@VARIANTNAME",  "气象仪_8"),
                new OleDbParameter("@HOUR_L", dt_start_hour),
                new OleDbParameter("@HOUR_H", dt_end_hour),
                new OleDbParameter("@MINUTE_L", dt_start_min),
                new OleDbParameter("@MINUTE_H", dt_end_min),
                new OleDbParameter("@SECOND_L", dt_start_sec),
                new OleDbParameter("@SECOND_H", dt_end_sec)
            };
            //发电量 逆变器1 1633发电量 
            OleDbParameter[] parameters_4_1 = {
                new OleDbParameter("@VARIANTNAME",  "nbq11_12"),
                new OleDbParameter("@HOUR_L", dt_start_hour),
                new OleDbParameter("@HOUR_H", dt_end_hour),
                new OleDbParameter("@MINUTE_L", dt_start_min),
                new OleDbParameter("@MINUTE_H", dt_end_min),
                new OleDbParameter("@SECOND_L", dt_start_sec),
                new OleDbParameter("@SECOND_H", dt_end_sec)
            };
            //发电量 逆变器2 1633发电量
            OleDbParameter[] parameters_4_2 = {
                new OleDbParameter("@VARIANTNAME",  "nbq21_12"),
                new OleDbParameter("@HOUR_L", dt_start_hour),
                new OleDbParameter("@HOUR_H", dt_end_hour),
                new OleDbParameter("@MINUTE_L", dt_start_min),
                new OleDbParameter("@MINUTE_H", dt_end_min),
                new OleDbParameter("@SECOND_L", dt_start_sec),
                new OleDbParameter("@SECOND_H", dt_end_sec)
            };
            //发电量 逆变器3 1633发电量
            OleDbParameter[] parameters_4_3 = {
                new OleDbParameter("@VARIANTNAME",  "nbq31_12"),
                new OleDbParameter("@HOUR_L", dt_start_hour),
                new OleDbParameter("@HOUR_H", dt_end_hour),
                new OleDbParameter("@MINUTE_L", dt_start_min),
                new OleDbParameter("@MINUTE_H", dt_end_min),
                new OleDbParameter("@SECOND_L", dt_start_sec),
                new OleDbParameter("@SECOND_H", dt_end_sec)
            };
            //取得平均值
            DataRow[] dr;
            try
            {
                DataRow[] dr_tmp = {
                               //平均值
                               AccessData.ExecuteDataRow(sql, AccessData.CONN_STRING1, parameters_1),
                               AccessData.ExecuteDataRow(sql, AccessData.CONN_STRING1, parameters_2),
                               AccessData.ExecuteDataRow(sql, AccessData.CONN_STRING1, parameters_3),
                               //发电量 20130603
                               AccessData.ExecuteDataRow(sql2, AccessData.CONN_STRING1, parameters_4_1),
                               AccessData.ExecuteDataRow(sql2, AccessData.CONN_STRING1, parameters_4_2),
                               AccessData.ExecuteDataRow(sql2, AccessData.CONN_STRING1, parameters_4_3)
                           };
                dr = dr_tmp;
            }
            catch (Exception ex)
            {
                LogManager.Logger.WriteWarnLog("数据库查询异常，查看数据库配置是否正确");
                return null;
            }
            Single[] s = new Single[6];
            for (int i = 0; i < 6; i++)
            {
                if (dr[i] != null)
                {
                    if (dr[i].IsNull(0))
                    {
                        s[i] = 0;
                    }
                    else
                    {
                        s[i] = Convert.ToSingle(dr[i][0]);
                    }
                }
                else
                {
                    s[i] = 0;
                }
            }
            return s;
            //存入发送数据库，准备发送 20130603
            //foreach (DataRow d in dr)
            //{
            //    if (d == null)
            //    {
            //        LogManager.Logger.WriteWarnLog("采集数据出错，四个指标不全");
            //        //d = new DataRow();
            //        //return null;
            //    }
            //}
            //return dr;
            //WriteToHisDb(FixedTime, dr, out insertedHisData);
        }

        public static void WriteToHisDb(DateTime FixedTime, /*DataRow[]*/ Single[] dr, out History_Data insertedHisData)
        {
            if (dr == null)
            {
                //
                insertedHisData = null;
                return;
            }
            string sql_dump = "insert into tbl_his_upload(ValueA, ValueB, ValueC, ValueD, timestamp_sendCycle, timestamp_upload, isupload) values(@ValueA, @ValueB, @ValueC, @ValueD, @timestamp_sendCycle, @timestamp_sendCycle, false)";
            OleDbParameter[] params_dump = { 
                                               new OleDbParameter("@ValueA", OleDbType.Single),
                                               new OleDbParameter("@ValueB", OleDbType.Single),
                                               new OleDbParameter("@ValueC", OleDbType.Single),
                                               new OleDbParameter("@ValueD", OleDbType.Single),
                                               new OleDbParameter("@timestamp_sendCycle", OleDbType.DBDate),
                                               new OleDbParameter("@timestamp_upload", OleDbType.DBDate)
                                           };
            params_dump[0].Value = (Single)dr[0];
            params_dump[1].Value = (Single)dr[1];
            params_dump[2].Value = (Single)dr[2];
            params_dump[3].Value = (Single)(dr[3] + dr[4] + dr[5]);
            params_dump[4].Value = FixedTime;
            params_dump[5].Value = FixedTime;
            int id;
            try
            {
                if ((id = AccessData.ExecuteNonQueryScalar(sql_dump, AccessData.CONN_STRING2, params_dump)) == 0)
                {
                    //未写成功
                    insertedHisData = null;
                    return;
                }
            }
            catch (Exception e)
            {
                LogManager.Logger.WriteWarnLog("写入数据库失败，查看数据库配置是否正确");
                insertedHisData = null;
                return;
            }
            insertedHisData = new History_Data();
            insertedHisData.id = id;
            insertedHisData.ValueA = dr[0];     //Convert.ToSingle(dr[0][0]);
            insertedHisData.ValueB = dr[1];     //Convert.ToSingle(dr[1][0]);
            insertedHisData.ValueC = dr[2];     //Convert.ToSingle(dr[2][0]);
            insertedHisData.ValueD = dr[3] + dr[4] + dr[5];     //Convert.ToSingle(dr[3][0]);
            insertedHisData.timestamp_sendCycle = FixedTime;
            insertedHisData.timestamp_upload = FixedTime;
            insertedHisData.isupload = false;
            //存入发送数据库，准备发送
        }
        /// <summary>
        /// id行数据发送成功后，更新其发送状态
        /// </summary>
        /// <param name="id"></param>
        public static void update_Upload(int id) 
        {
            string sql_update = "update tbl_his_upload set isupload = true, timestamp_upload = @timestamp_upload where id = @id";
            DateTime dt_now = DateTime.Now.ToLocalTime();
            OleDbParameter[] params_u = new OleDbParameter[2];

            OleDbParameter[] params_update = { 
                                                new OleDbParameter("@timestamp_upload", dt_now.ToString()),
                                                new OleDbParameter("@id", id)
                                             };
            try
            {
                AccessData.ExecuteNonQuery(sql_update, AccessData.CONN_STRING2, params_update);
            }
            catch (Exception e)
            {
                LogManager.Logger.WriteWarnLog("更新数据库失败，查看数据库配置是否正确");
            }
        }
        /// <summary>
        /// 区间内失败历史数据集合
        /// </summary>
        /// <param name="dt_begin"></param>
        /// <param name="dt_end"></param>
        /// <returns></returns>
        public static History_Data[] FetchDataFail(DateTime dt_begin, DateTime dt_end)
        {
            //string sql_region = "select id, ValueA, ValueB, ValueC, ValueD from tbl_his_upload where timestamp_sendCycle >= @dt_begin and timestamp_sendCycle <= @dt_end and isupload=false";
            string sql_region = "select id, ValueA, ValueB, ValueC, ValueD from tbl_his_upload where timestamp_sendCycle between @dt_begin and  @dt_end and isupload=false";
            OleDbParameter[] params_region = {
                                                 new OleDbParameter("@dt_begin", dt_begin),
                                                 new OleDbParameter("@dt_end", dt_end),
                                             };
            
            DataTable dtbl;
            try
            {
                dtbl = AccessData.ExecuteDataTable(sql_region, AccessData.CONN_STRING2, params_region);
            }
            catch (Exception ex)
            {
                return null; 
            }
            if(dtbl == null)
            {
                return null;
            }
            History_Data[] arrary_hd = new History_Data[dtbl.Rows.Count];
            int i = 0;
            foreach (DataRow dr in dtbl.Rows)
            {
                History_Data hd = new History_Data();
                hd.id = Convert.ToInt32(dr[0]);
                hd.ValueA = Convert.ToSingle(dr[1]);
                hd.ValueB = Convert.ToSingle(dr[2]);
                hd.ValueC = Convert.ToSingle(dr[3]);
                hd.ValueD = Convert.ToSingle(dr[4]);
                arrary_hd[i] = hd;
                i++;   
            }
            return arrary_hd;
            //ds = AccessData.ExecuteDataset("", AccessData.CONN_STRING2);
        }
        /// <summary>
        /// 区间内成功发送集合
        /// </summary>
        /// <param name="dt_begin"></param>
        /// <param name="dt_end"></param>
        /// <returns></returns>
        public static History_Data[] FetchDataSuccess(DateTime dt_begin, DateTime dt_end)
        {
            //string sql_region = "select id, ValueA, ValueB, ValueC, ValueD from tbl_his_upload where timestamp_sendCycle >= @dt_begin and timestamp_sendCycle <= @dt_end and isupload=true";
            string sql_region = "select id, ValueA, ValueB, ValueC, ValueD from tbl_his_upload where timestamp_sendCycle between @dt_begin and  @dt_end and isupload=true";
            OleDbParameter[] params_region = {
                                                 new OleDbParameter("@dt_begin", dt_begin),
                                                 new OleDbParameter("@dt_end", dt_end),
                                             };
            DataTable dtbl;
            try
            {
                dtbl = AccessData.ExecuteDataTable(sql_region, AccessData.CONN_STRING2, params_region);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (dtbl == null)
            {
                return null;
            }
            History_Data[] arrary_hd = new History_Data[dtbl.Rows.Count];
            int i = 0;
            foreach (DataRow dr in dtbl.Rows)
            {
                History_Data hd = new History_Data();
                hd.id = Convert.ToInt32(dr[0]);
                hd.ValueA = Convert.ToSingle(dr[1]);
                hd.ValueB = Convert.ToSingle(dr[2]);
                hd.ValueC = Convert.ToSingle(dr[3]);
                hd.ValueD = Convert.ToSingle(dr[4]);
                arrary_hd[i] = hd;
                i++;
            }
            return arrary_hd;
            //ds = AccessData.ExecuteDataset("", AccessData.CONN_STRING2);
        }
    }
    /// <summary>
    /// 采集数据库映射类
    /// </summary>
    public class Monitor_Data
    {
        public int ONLYNUM;
        public string VARIANTNAME;
        public int HOURF;
        public int MINUTEF;
        public int SECONDF;
        public int TYPE;
        public int DATA;
    }
    /// <summary>
    /// 数据库历史数据表映射类
    /// </summary>
    public class History_Data
    {
        /// <summary>
        /// id字段
        /// </summary>
        public int id;
        /// <summary>
        /// 平行于光伏组件的太阳辐照度 总辐射
        /// </summary>
        public float ValueA;
        /// <summary>
        /// 室外温度 大气温度
        /// </summary>
        public float ValueB;
        /// <summary>
        /// 光伏组件背面表面温度 大地温度
        /// </summary>
        public float ValueC;
        /// <summary>
        /// 发电量 1633发电量
        /// </summary>
        public float ValueD;
        /// <summary>
        /// 定时发送时刻时间戳
        /// </summary>
        public DateTime timestamp_sendCycle;
        /// <summary>
        /// 成功上传时间
        /// </summary>
        public DateTime timestamp_upload;
        /// <summary>
        /// 是否已上传
        /// </summary>
        public bool isupload;


    }
}
