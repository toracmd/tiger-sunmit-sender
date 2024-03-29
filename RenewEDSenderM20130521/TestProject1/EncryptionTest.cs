﻿using RenewEDSenderM.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///这是 EncryptionTest 的测试类，旨在
    ///包含所有 EncryptionTest 单元测试
    ///</summary>
    [TestClass()]
    public class EncryptionTest
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
        ///Encryption 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void EncryptionConstructorTest()
        {
            Encryption target = new Encryption();
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }

        /// <summary>
        ///AES_Encrypt 的测试
        ///</summary>
        [TestMethod()]
        public void AES_EncryptTest()
        {
            Encryption.AES_Encrypt();
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///CRC16 的测试
        ///</summary>
        [TestMethod()]
        public void CRC16Test()
        {
            byte[] buffer = null; // TODO: 初始化为适当的值
            int expected = 0; // TODO: 初始化为适当的值
            int actual;
            actual = Encryption.CRC16(buffer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///MD5_Encrypt 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("RenewEDSenderM.exe")]
        public void MD5_EncryptTest()
        {
            byte[] data = null; // TODO: 初始化为适当的值
            byte[] expected = null; // TODO: 初始化为适当的值
            byte[] actual;
            actual = Encryption_Accessor.MD5_Encrypt(data);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///getMd5Hash 的测试
        ///</summary>
        [TestMethod()]
        public void getMd5HashTest()
        {
            string input = string.Empty; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            actual = Encryption.getMd5Hash(input);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///verifyMd5Hash 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("RenewEDSenderM.exe")]
        public void verifyMd5HashTest()
        {
            string input = string.Empty; // TODO: 初始化为适当的值
            string hash = string.Empty; // TODO: 初始化为适当的值
            bool expected = false; // TODO: 初始化为适当的值
            bool actual;
            actual = Encryption_Accessor.verifyMd5Hash(input, hash);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
