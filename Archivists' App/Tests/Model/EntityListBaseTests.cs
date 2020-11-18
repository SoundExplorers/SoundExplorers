using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
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

    private class DudErrorThrowerList : NamedEntityList<ErrorThrowingEventType> {
      [ExcludeFromCodeCoverage]
      public override IList GetChildrenForMainList(int rowIndex) {
        throw new NotSupportedException();
      }
    }

    private TestSession Session { get; set; }

    [Test]
    public void RethrowDudError() {
      var list = new DudErrorThrowerList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor =
        new TestEditor<ErrorThrowingEventType, NamedBindingItem<ErrorThrowingEventType>>(
          list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = "Dudley";
      Assert.Throws<InvalidOperationException>(() => list.OnRowValidated(0));
    }

    [Test]
    public void GetChildrenForMainListNotSupported() {
      var list = new ActList();
      Assert.Throws<NotSupportedException>(
        () => list.GetChildrenForMainList(0));
    }
  }
}