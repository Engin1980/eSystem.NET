using ESystem.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Collections;

namespace ESystemTest
{
    
    
    /// <summary>
    ///This is a test class for ICollectionExtTest and is intended
    ///to contain all ICollectionExtTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ICollectionExtTest
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

    private List<int> GetList()
    {
      List<int> lst = new List<int>();
      lst.Add(1);
      lst.Add(2);
      lst.Add(3);
      lst.Add(4);
      return lst;
    }

    /// <summary>
    ///A test for ToString
    ///</summary>
    [TestMethod()]
    public void ToStringTest()
    {
      ICollection arr = GetList();
      string separator = ";";
      string expected = "1;2;3;4";
      string actual;
      actual = arr.ToString(separator);
      EAssert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void SplitByIndexTest()
    {
      IList<int> coll = GetList();
      int index = 2;
      bool excludeSplittingItem = true;
      List<int>[] expected = new List<int>[] { new List<int>() { 1, 2 }, new List<int>() { 4 } };
      List<int>[] actual;
      actual = coll.SplitByIndex<int>(index, excludeSplittingItem);

      EAssert.AreEqual(actual, expected);
    }

    [TestMethod()]
    public void RemoveRangeTest()
    {
      ICollection<int> coll = GetList();
      IEnumerable<int> items = new List<int> { 1, 3 };
      IEnumerable<int> expected = new List<int> { 2, 4 };
      ICollectionExt.RemoveRange<int>(coll, items);
      EAssert.AreEqual(coll, expected);
    }

    /// <summary>
    ///A test for LastIndex
    ///</summary>
    [TestMethod()]
    public void LastIndexTest()
    {
      ICollection arr = GetList();
      int expected = 3;
      int actual;
      actual = ICollectionExt.LastIndex(arr);
      EAssert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void FindAllTest()
    {
      List<int> coll = GetList();
      Func<int, bool> predicate = (int i) => (i > 1);
      List<int> expected = new List<int>() { 2, 3, 4 };
      List<int> actual;
      actual = ICollectionExt.FindAll<int>(coll, predicate);
      EAssert.AreEqual(actual, expected);
      EAssert.AreEqual(coll.Count, 4);
    }

    [TestMethod()]
    public void EGetRangeTest1()
    {
      IList<int> coll = GetList();
      int startIndex = 2;
      List<int> expected = new List<int> { 3, 4 };
      List<int> actual;
      actual = IListExt.EGetRange<int>(coll, startIndex);
      EAssert.AreEqual(actual, expected);
    }

    [TestMethod()]
    public void EGetRangeTest()
    {
      IList<int> coll = GetList();
      int startIndex = 1;
      int count = 2;
      List<int> expected = new List<int>() { 2, 3 };
      List<int> actual;
      actual = IListExt.EGetRange<int>(coll, startIndex, count);
      EAssert.AreEqual(actual, expected);
    }

    [TestMethod()]
    public void CutToTest()
    {
      List<int> coll = GetList();
      ICollection<int> targetCollection = new List<int>();
      Func<int, bool> predicate = (int i) => (i > 1);
      List<int> expected = new List<int>() { 2, 3, 4 };
      List<int> actual;
      actual = ICollectionExt.CutTo<int>(coll, targetCollection, predicate);
      EAssert.AreEqual(actual, expected);
      EAssert.AreEqual(coll.Count, 1);
    }

    [TestMethod()]
    public void CutTest()
    {
      List<int> coll = GetList();
      Func<int, bool> predicate = (int i) => (i > 1);
      List<int> expected = new List<int>() { 2, 3, 4 };
      List<int> actual;
      actual = ICollectionExt.Cut<int>(coll, predicate);
      EAssert.AreEqual(actual, expected);
      EAssert.AreEqual(coll.Count, 1);
    }
  }
}
