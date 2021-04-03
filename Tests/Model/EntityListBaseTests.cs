using System;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class EntityListBaseTests : TestFixtureBase {
    private class DudErrorThrowerList : NamedEntityList<ErrorThrower> { }

    [Test]
    public void RethrowDudError() {
      var list = new DudErrorThrowerList {
        QueryHelper = new QueryHelper(),
        Session = Session
      };
      list.Populate(); // Creates an empty BindingList
      var bindingList = list.BindingList;
      var item1 = bindingList.AddNew();
      list.OnRowEnter(0);
      item1.Name = "Dudley";
      Assert.Throws<InvalidOperationException>(() => list.OnRowValidated(0));
    }
  }
}