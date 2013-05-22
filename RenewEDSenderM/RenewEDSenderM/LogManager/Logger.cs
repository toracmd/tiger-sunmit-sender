using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using log4net.Core;

namespace RenewEDSenderM.LogManager
{
    /// <summary>
    /// @class 方法信息类
    /// @brief 被调用方法的信息
    /// </summary>
    class InfoMethod
    {
        /// <summary>
        /// 有关方法和构造函数信息
        /// </summary>
        private MethodBase method;
        /// <summary>
        /// 方法的源文件名
        /// </summary>
        private String fileName;
        /// <summary>
        /// 方法在源文件的行号
        /// </summary>
        private int lineNum;
        /// <summary>
        /// 方法在源文件的列号
        /// </summary>
        private int columnNum;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="m">方法信息</param>
        /// <param name="s">文件名</param>
        /// <param name="l">调用行</param>
        /// <param name="c">调用列</param>
        public InfoMethod(MethodBase m, String s, int l, int c)
        {
            method = m;
            fileName = s;
            lineNum = l;
            columnNum = c;
        }
        /// <summary>
        /// 获得方法成员
        /// </summary>
        /// <returns></returns>
        public MethodBase getMethod()
        {
            return method;
        }
        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <returns></returns>
        public String getFileName()
        {
            return fileName;
        }
        /// <summary>
        /// 获得被调用所在行
        /// </summary>
        /// <returns></returns>
        public int getLineNum()
        {
            return lineNum;
        }
        /// <summary>
        /// 获得被调用所在列
        /// </summary>
        /// <returns></returns>
        public int getColumnNum()
        {
            return columnNum;
        }

    }
    /// <summary>
    /// @class Logger
    /// 1、由配置文件app.config中log4net段控制
    /// 2、log输出模式为Rollingbackup方式，以日期文件夹和日志回滚文件大小区分
    /// 3、用了单体模式
    /// </summary>
    class Logger
    {
        /// <summary>
        /// 函数入口标志
        /// </summary>
        public static readonly String FUN_ENTRY = "[Enter]";
        /// <summary>
        /// 函数出口标志
        /// </summary>
        public static readonly String FUN_EXIT = "[Exit]";
        #region 单体模式
        /// <summary>
        /// Logger实体成员instance
        /// </summary>
        private static Logger instance = new Logger();
        /// <summary>
        /// log4net.ILog对象
        /// </summary>
        private log4net.ILog logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        private Logger()
        {
            logger = log4net.LogManager.GetLogger("AppSender.Logging");
        }
        /// <summary>
        /// 获得一个实体
        /// </summary>
        /// <returns></returns>
        public static Logger getInstance()
        {
            return instance;
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// Log输出共同方法
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="hierarchy">调用层次</param>
        /// <param name="format">输出格式</param>
        /// <param name="args">输出参数</param>
        private static void LogByLevel(Level level, int hierarchy, String format, params object[] args)
        {
            // 调用层次+1
            hierarchy += 1;
            // 从堆栈中获取该方法信息
            InfoMethod info = getMethodFromStack(hierarchy);
            // 函数调用信息[FileName][Line,Column:][type][class]
            String strHeadFmt = String.Format("[{0}:{1},{2},[{3}][{4}]",
                info.getFileName(),
                info.getLineNum(),
                info.getColumnNum(),
                info.getMethod().DeclaringType,
                info.getMethod().Name);
            strHeadFmt += format;
            // 输出Debug级别Log
            if (level.Equals(Level.Debug))
            {
                getInstance().logger.DebugFormat(strHeadFmt, args);
            }
            // 输出Info级别Log
            else if (level.Equals(Level.Info))
            {
                getInstance().logger.InfoFormat(strHeadFmt, args);
            }
            // 输出Warn级别Log
            else if (level.Equals(Level.Warn))
            {
                getInstance().logger.WarnFormat(strHeadFmt, args);
            }
            // 输出Error级别Log
            else if (level.Equals(Level.Error))
            {
                getInstance().logger.ErrorFormat(strHeadFmt, args);
            }
            // 输出Fatal级别Log
            else if (level.Equals(Level.Fatal))
            {
                getInstance().logger.FatalFormat(strHeadFmt, args);
            }
            // 默认输出Info级别Log
            else
            {
                getInstance().logger.InfoFormat(strHeadFmt, args);
            }  
        }
        /// <summary>
        /// 从堆栈中获取方法信息
        /// </summary>
        /// <param name="hierarchy">调用层次</param>
        /// <returns></returns>
        private static InfoMethod getMethodFromStack(int hierarchy)
        {
            hierarchy += 1;
            // 获取堆栈,构造函数参数true输出源文件行列信息否则不输出
            StackTrace strace = new StackTrace(false);
            // 判断堆栈中总帧数
            int nFrameIndex = strace.FrameCount > hierarchy ? hierarchy : 0;
            // 从堆栈中获取当前帧
            StackFrame stackFrame = strace.GetFrame(nFrameIndex);
            // 被调用方法信息
            MethodBase method = strace.GetFrame(nFrameIndex).GetMethod();
            // 调用处信息
            InfoMethod infoMethod = new InfoMethod(method,
                stackFrame.GetFileName(),
                stackFrame.GetFileLineNumber(),
                stackFrame.GetFileColumnNumber());
            return infoMethod;
            
        }
        #endregion
        #region 公开方法
        /// <summary>
        /// 写Info级别Log
        /// </summary>
        /// <param name="format">格式化输出串</param>
        /// <param name="args">参数</param>
        public static void WriteInfoLog(String format, params object[] args)
        {
            LogByLevel(Level.Info, 1, format, args);
        }
        /// <summary>
        /// 写Debug级别Log
        /// </summary>
        /// <param name="format">格式化输出串</param>
        /// <param name="args">参数</param>
        public static void WriteDebugLog(String format, params object[] args)
        {
            LogByLevel(Level.Debug, 1, format, args);
        }
        /// <summary>
        /// 写Warn级别Log
        /// </summary>
        /// <param name="format">格式化输出串</param>
        /// <param name="args">参数</param>
        public static void WriteWarnLog(String format, params object[] args)
        {
            LogByLevel(Level.Warn, 1, format, args);
        }
        /// <summary>
        /// 写Error级别Log
        /// </summary>
        /// <param name="format">格式化输出串</param>
        /// <param name="args">参数</param>
        public static void WriteErrorLog(String format, params object[] args)
        {
            LogByLevel(Level.Error, 1, format, args);
        }
        /// <summary>
        /// 写Fatal级别Log
        /// </summary>
        /// <param name="format">格式化输出串</param>
        /// <param name="args">参数</param>
        public static void WriteFatalLog(String format, params object[] args)
        {
            LogByLevel(Level.Fatal, 1, format, args);
        }
        /// <summary>  
        /// 函数入口日志，Info级别  
        /// </summary>  
        /// <param name="args"></param> 
        public static void FuncEntryLog(params object[] args)
        {
            StringBuilder sbMsg = new StringBuilder("Function Enterd...(");
            // 解析参数
            for (int i = 0; i < args.Length; i++)
            {
                sbMsg.Append("<arg");
                sbMsg.Append(i);
                sbMsg.Append(":");
                sbMsg.Append(args[i]);
                sbMsg.Append(">");
                if (i != args.Length - 1)
                {
                    sbMsg.Append(", ");
                }
            }
            sbMsg.Append(")");  
            LogByLevel(Level.Info, 1, sbMsg.ToString());
        }
        /// <summary>
        /// 函数退出日志，Info级别  
        /// </summary>
        public static void FuncExitLog()
        {
            LogByLevel(Level.Info, 1, "Function Exited...");
        }
        #endregion
    }
}
