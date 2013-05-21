using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MySql.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

using System.Data.OleDb;
using System.Data.Common;


/// <summary>
///Connect类的摘要说明
/// </summary>
namespace RenewEDSenderM.DbManager
{
    /// <summary>
    /// 访问MySQL数据库类
    /// </summary>
    public class Connect
    {
        #region 成员变量
        //静态成员变量：数据库连接字符串，定义在app.config文件
        private static readonly string conStr = ConfigurationManager.AppSettings["ConnDBstring"];
        //定义一个Mqsql连接对象并置空
        public MySqlConnection conn;
        #endregion
        #region 成员函数
        /// <summary>
        ///  构造函数，默认是把App.config中的数据库连接字符串直接给成员变量conn当参数，即每一个实例默认都连到指定数据库
        /// </summary>
        public Connect()
        {
            LogManager.Logger.FuncEntryLog();
            LogManager.Logger.WriteInfoLog("Class Connect Construct Begin!");
            try
            {
                conn = new MySqlConnection(conStr);
            }
            catch (Exception ex)
            {
                LogManager.Logger.WriteErrorLog("{0}", ex);
                conn = null;
            }
            LogManager.Logger.FuncExitLog();
        }

        /// <summary>
        ///  给定连接的数据库用假设参数执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <returns>执行命令所影响的行数,如果连接数据库失败返回-1</returns>
        public int ExecuteSql(string cmdText)
        {
            LogManager.Logger.FuncEntryLog(cmdText);
            if (conn == null)
            {
                LogManager.Logger.WriteErrorLog("Connect DB failed!");
                return -1;
            }
            if (conn.State != ConnectionState.Open)
                conn.Open();
            int irows;
            MySqlCommand cmd = new MySqlCommand(cmdText);
            cmd.Connection = conn;
            irows = cmd.ExecuteNonQuery();
            LogManager.Logger.WriteDebugLog("The number of dataset is {0}", irows);
            LogManager.Logger.FuncExitLog();
            return irows;
        }

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的,连接数据库失败返回空</returns>
        public object GetSingle(string cmdText)
        {
            LogManager.Logger.FuncEntryLog(cmdText);
            if (conn == null)
            {
                LogManager.Logger.WriteErrorLog("Connect DB failed!");
                return null;
            }
            if (conn.State != ConnectionState.Open)
                conn.Open();
            object single;
            MySqlCommand cmd = new MySqlCommand(cmdText);
            cmd.Connection = conn;
            single = cmd.ExecuteScalar();
            LogManager.Logger.WriteDebugLog("The number of dataset is {0}", single);
            LogManager.Logger.FuncExitLog();
            return single;
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <returns>返回select语句选中的数据集，连接失败返回空</returns>
        public DataSet GetDataSet(string cmdText)
        {
            LogManager.Logger.FuncEntryLog(cmdText);
            if (conn == null)
            {
                LogManager.Logger.WriteErrorLog("Connect DB failed!");
                return null;
            }
            if (conn.State != ConnectionState.Open)
                conn.Open();
            DataSet dataset = new DataSet();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand(cmdText);
            cmd.Connection = conn;
            adapter.SelectCommand = cmd;
            adapter.Fill(dataset);
            LogManager.Logger.WriteDebugLog("Have get the dataset！");
            LogManager.Logger.FuncExitLog();
            return dataset;
        }

        /// <summary>
        /// Connect对象打开连接数据库；
        /// </summary>
        public void Open()
        {
            LogManager.Logger.FuncEntryLog();
            conn.Open();
            LogManager.Logger.FuncExitLog();
        }

        /// <summary>
        /// Connect对象关闭连接数据库；
        /// </summary>
        public void Close()
        {
            LogManager.Logger.FuncEntryLog();
            conn.Close();
            LogManager.Logger.FuncExitLog();
        }
        #endregion
    }
}
