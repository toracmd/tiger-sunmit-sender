using RenewEDSenderM.LogManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using log4net.Core;

namespace TestProject1
{
    
    
    /// <summary>
    ///这是 LoggerTest 的测试类，旨在
    ///包含所有 LoggerTest 单元测试
    ///</summary>
    [TestClass()]
    public class LoggerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Logger 构造函数 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("RenewEDSenderM.exe")]
        public void LoggerConstructorTest()
        {
            Logger_Accessor target = new Logger_Accessor();
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }

        /// <summary>
        ///FuncEntryLog 的测试
        ///</summary>
        [TestMethod()]
        public void FuncEntryLogTest()
        {
            object[] args = null; // TODO: 初始化为适当的值
            Logger.FuncEntryLog(args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///FuncExitLog 的测试
        ///</summary>
        [TestMethod()]
        public void FuncExitLogTest()
        {
            Logger.FuncExitLog();
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///LogByLevel 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("RenewEDSenderM.exe")]
        public void LogByLevelTest()
        {
            Level level = null; // TODO: 初始化为适当的值
            int hierarchy = 0; // TODO: 初始化为适当的值
            string format = string.Empty; // TODO: 初始化为适当的值
            object[] args = null; // TODO: 初始化为适当的值
            Logger_Accessor.LogByLevel(level, hierarchy, format, args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///WriteDebugLog 的测试
        ///</summary>
        [TestMethod()]
        public void WriteDebugLogTest()
        {
            string format = string.Empty; // TODO: 初始化为适当的值
            object[] args = null; // TODO: 初始化为适当的值
            Logger.WriteDebugLog(format, args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///WriteErrorLog 的测试
        ///</summary>
        [TestMethod()]
        public void WriteErrorLogTest()
        {
            string format = string.Empty; // TODO: 初始化为适当的值
            object[] args = null; // TODO: 初始化为适当的值
            Logger.WriteErrorLog(format, args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///WriteFatalLog 的测试
        ///</summary>
        [TestMethod()]
        public void WriteFatalLogTest()
        {
            string format = string.Empty; // TODO: 初始化为适当的值
            object[] args = null; // TODO: 初始化为适当的值
            Logger.WriteFatalLog(format, args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///WriteInfoLog 的测试
        ///</summary>
        [TestMethod()]
        public void WriteInfoLogTest()
        {
            string format = string.Empty; // TODO: 初始化为适当的值
            object[] args = null; // TODO: 初始化为适当的值
            Logger.WriteInfoLog(format, args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///WriteWarnLog 的测试
        ///</summary>
        [TestMethod()]
        public void WriteWarnLogTest()
        {
            string format = string.Empty; // TODO: 初始化为适当的值
            object[] args = null; // TODO: 初始化为适当的值
            Logger.WriteWarnLog(format, args);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///getInstance 的测试
        ///</summary>
        [TestMethod()]
        public void getInstanceTest()
        {
            Logger expected = null; // TODO: 初始化为适当的值
            Logger actual;
            actual = Logger.getInstance();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///getMethodFromStack 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("RenewEDSenderM.exe")]
        public void getMethodFromStackTest()
        {
            int hierarchy = 0; // TODO: 初始化为适当的值
            InfoMethod_Accessor expected = null; // TODO: 初始化为适当的值
            InfoMethod_Accessor actual;
            actual = Logger_Accessor.getMethodFromStack(hierarchy);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
