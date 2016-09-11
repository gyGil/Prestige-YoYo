/// \file UnitTest1.cs
///
/// \mainpage PROG3070 - Advanced SQL
///
/// \section intro Program Introduction
/// - This is the Unit Test solution that tests out all the various functions of DAL class
///
/// Major <b>UnitTestEMS.cs</b>
/// \section version Current version of this Program
/// <ul>
/// <li>\author         Marcus Rankin, GeunYoung Gil</li>
/// <li>\references     N/A
/// <li>\version        1.00.00</li>
/// <li>\date           2016.03.17</li>
/// <li>\pre            N/A
/// <li>\warning        N/A
/// <li>\copyright      Marcus Rankin, GeunYoung Gil</li>
/// <ul>
/// 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DALlib;

namespace HarnessTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// UnitTest 01 : TestDALClass_OpenConnection_Normal()
        /// Purpose: If the OpenConnection() method can set the valid value to true
        /// Test Conducted: Automatic
        /// Type of Test: Normal Test
        /// Expected outcome: is true
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_OpenConnection_Normal()
        {
            DAL dal = new DAL();
            Assert.IsTrue(dal.OpenConnection("sa", "Conestoga1"));
        }

        /// <summary>
        /// UnitTest 02 : TestDALClass_OpenConnection_Exceptional()
        /// Purpose: If the OpenConnection() method can set the valid value to true
        /// Test Conducted: Automatic
        /// Type of Test: Exception Test
        /// Expected outcome: False
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_OpenConnection_Exceptional_1()
        {
            DAL dal = new DAL();
            Assert.IsFalse(dal.OpenConnection("", ""));
        }

        /// <summary>
        /// UnitTest 03 : TestDALClass_OpenConnection_Exceptional()
        /// Purpose: If the OpenConnection() method can set the valid value to true
        /// Test Conducted: Automatic
        /// Type of Test: Exception Test
        /// Expected outcome: False
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_OpenConnection_Exceptional_2()
        {
            DAL dal = new DAL();
            Assert.IsFalse(dal.OpenConnection("NonProperId", "NonPassword"));
        }

        /// <summary>
        /// UnitTest 04 : TestDALClass_addManufacturingData_Normal_1()
        /// Purpose: If the addManufacturingData() method can set the valid value to true
        /// Test Conducted: Automatic
        /// Type of Test: Normal Test
        /// Expected outcome: True
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_addManufacturingData_Normal_1()
        {
            DAL dal = new DAL();
            String inputStr = @"WorkArea, 2aa3984c-5007-41fa-b655-538d011b9bbf, Line0, INSPECTION_1,,3/15/2016 3:17:46 PM";
            dal.OpenConnection("sa", "Conestoga1");
            Assert.AreEqual(1, dal.AddManufacturingData(inputStr));
        }

        /// <summary>
        /// UnitTest 05 : TestDALClass_addManufacturingData_Normal_2()
        /// Purpose: If the addManufacturingData() method can set the valid value to true
        /// Test Conducted: Automatic
        /// Type of Test: Normal Test
        /// Expected outcome: True
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_addManufacturingData_Normal_2()
        {
            DAL dal = new DAL();
            String inputStr = @"WorkArea, 2ab3984c-1004-41fa-b655-538d011b9bbk, Line0, INSPECTION_3,Broken shell,3/15/2016 3:17:46 PM";
            dal.OpenConnection("sa", "Conestoga1");
            Assert.AreEqual(1, dal.AddManufacturingData(inputStr));
        }

        /// <summary>
        /// UnitTest 06 : TestDALClass_addManufacturingData_Boundary_1()
        /// Purpose: If the addManufacturingData() method can set the valid value to true
        /// Test Conducted: Automatic
        /// Type of Test: Boundary Test
        /// Expected outcome: True
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_addManufacturingData_Boundary_1()
        {
            DAL dal = new DAL();
            String inputStr = @",,,,,";
            dal.OpenConnection("sa", "Conestoga1");
            Assert.AreEqual(1, dal.AddManufacturingData(inputStr));
        }

        /// <summary>
        /// UnitTest 07 : TestDALClass_addManufacturingData_Boundary_2()
        /// Purpose: If the addManufacturingData() method accept a argument has more commas 
        /// Test Conducted: Automatic
        /// Type of Test: Boundary Test
        /// Expected outcome: True
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_addManufacturingData_Boundary_2()
        {
            DAL dal = new DAL();
            String inputStr = @"Boundary_test, 2aa3984c-5007-41fa-b655-538d011b9bbf, Line0, INSPECTION_1,,3/15/2016 3:17:46 PM,Extra value";
            dal.OpenConnection("sa", "Conestoga1");
            Assert.AreEqual(1, dal.AddManufacturingData(inputStr));
        }

        /// <summary>
        /// UnitTest 08 : TestDALClass_AddDataToDatabase_Normal()
        /// Purpose: If the AddDataToDatabase() method can set the valid value
        /// Test Conducted: Automatic
        /// Type of Test: Normal Test
        /// Expected outcome: 0
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_AddDataToDatabase_Normal()
        {
            DAL dal = new DAL();
            dal.OpenConnection("sa", "Conestoga1");
            Assert.AreEqual(0, dal.AddDataToDatabase(0));
        }


        /// <summary>
        /// UnitTest 09 : TestDALClass_CloseConnection_Normal_1()
        /// Purpose: If the CloseConnection() method can close proper way.
        ///          If it is successfully closed return false.
        /// Test Conducted: Automatic
        /// Type of Test: Normal Test
        /// Expected outcome: False
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_CloseConnection_Normal_1()
        {
            DAL dal = new DAL();
            dal.OpenConnection("sa", "Conestoga1");
            Assert.IsFalse(dal.CloseConnection());
        }

        /// <summary>
        /// UnitTest 10 : TestDALClass_CloseConnection_Exception_1()
        /// Purpose: If the CloseConnection() method can close proper way.
        ///          If it is successfully closed return false.
        /// Test Conducted: Automatic
        /// Type of Test: Normal Test
        /// Expected outcome: True
        /// Acutal Test Result: Pass
        /// </summary>
        [TestMethod]
        public void TestDALClass_CloseConnection_Exception_1()
        {
            DAL dal = new DAL();
            Assert.IsTrue(dal.CloseConnection());
        }
    }

}