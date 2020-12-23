using System;
using NUnit.Framework;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class BindingColumnListTests {
    [Test]
    public void DisallowAddDuplicate() {
      const string columnName = "Description";
      var column = new BindingColumn(columnName);
      var duplicate = new BindingColumn(columnName);
      var list = new BindingColumnList {column};
      Assert.Throws<ArgumentException>(() => list.Add(duplicate));
    }
  }
}