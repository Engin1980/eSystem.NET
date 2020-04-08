using ESystem.Math.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ESystem.Math.Matrices_Test
{
    
    
    /// <summary>
    ///This is a test class for ArrayOfArrayMatrixTest and is intended
    ///to contain all ArrayOfArrayMatrixTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ArrayOfArrayMatrixTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
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

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for ArrayOfArrayMatrix Constructor
    ///</summary>
    [TestMethod()]
    public void ArrayOfArrayMatrixConstructorTest()
    {
      double[][] value = new double[2][];
      value[0] = new double[] { 1, 2, 3 };
      value[1] = new double[] { 4, 5, 6 };
      ArrayOfArrayMatrix target = new ArrayOfArrayMatrix(value);
    }
  }
}
