using ESystem.Math.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ESystem.Math.Matrices_Test
{
    
    
    /// <summary>
    ///This is a test class for MultidimensionalArrayMatrixTest and is intended
    ///to contain all MultidimensionalArrayMatrixTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MultidimensionalArrayMatrixTest
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
    ///A test for MultidimensionalArrayMatrix Constructor
    ///</summary>
    [TestMethod()]
    [DeploymentItem("EMatrix.dll")]
    public void MultidimensionalArrayMatrixConstructorTest()
    {
      int rowCount = 2; 
      int columnCount = 3; 
      MultidimensionalArrayMatrix target = new MultidimensionalArrayMatrix(rowCount, columnCount);
      target[1, 1] = 0;
      target[1, 2] = 0;
      target[1, 3] = 0;
      target[2, 1] = 0;
      target[2, 2] = 0;
      target[2, 3] = 0;

      try
      {
        target[3, 1] = 0;
        Assert.Fail("Dimension went out of matrix range without error.");
      }
      catch (EMatrixException) { }

      try
      {
        target[2, 4] = 0;
        Assert.Fail("Dimension went out of matrix range without error.");
      }
      catch (EMatrixException) { }
    }
  }
}
