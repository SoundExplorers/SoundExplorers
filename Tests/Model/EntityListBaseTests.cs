using System;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class EntityListBaseTests {
    [SetUp]
    public void Setup() {
      Session = new TestSession();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private class DudErrorThrowerList : NamedEntityList<ErrorThrower> {
      protected override SortedEntityCollection<ErrorThrower>
        FetchOrAddListRoot() {
        Session.BeginUpdate();
        var result =
          QueryHelper.FindSingleton<SortedEntityCollection<ErrorThrower>>(
            Session);
        if (result == null) {
          result = new SortedEntityCollection<ErrorThrower>();
          Session.Persist(result);
        }
        Session.Commit();
        return result;
      }
    }

    private TestSession Session { get; set; } = null!;

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