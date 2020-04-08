using ESystem.Math.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ESystem.Math.Matrices_Test
{


  /// <summary>
  ///This is a test class for MatrixTest and is intended
  ///to contain all MatrixTest Unit Tests
  ///</summary>
  [TestClass()]
  public class MatrixTest
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
    ///A test for AreEqual
    ///</summary>
    [TestMethod()]
    public void AreEqualTest()
    {
      Matrix a = new SingleArrayMatrix(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4);
      Matrix b = new MultidimensionalArrayMatrix(new double[,] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } });      
      double[][] pom = new double[2][];
      pom[0] = new double[] { 1, 2, 3, 4 };
      pom[1] = new double[] { 5, 6, 7, 8 };
      Matrix c = new ArrayOfArrayMatrix(pom);

      bool actual;
      actual = Matrix.AreEqual(a, b);
      Assert.IsTrue(actual, "Matrices A and B are different.");
      actual = Matrix.AreEqual(a, c);
      Assert.IsTrue(actual, "Matrice A and C are different.");
      actual = Matrix.AreEqual(b, c);
      Assert.IsTrue(actual, "Matrice B and C are different.");      
    }

    internal virtual Matrix CreateMatrix()
    {
      // TODO: Instantiate an appropriate concrete class.
      Matrix target = null;
      return target;
    }

    /// <summary>
    ///A test for Transpond
    ///</summary>
    [TestMethod()]
    public void TranspondTest()
    {
      Matrix target = new SingleArrayMatrix(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4);
      Matrix expected = new SingleArrayMatrix(new double[] { 1, 5, 2, 6, 3, 7, 4, 8 }, 2);
      Matrix actual;
      actual = target.Transpond();
      Assert.IsTrue(Matrix.AreEqual(actual, expected));
    }
  }
}
