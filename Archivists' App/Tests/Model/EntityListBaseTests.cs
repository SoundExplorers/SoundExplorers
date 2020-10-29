using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
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

    private class DudErrorThrower : EventType {
      [UsedImplicitly]
      public new string Name {
        get => SimpleKey;
        set => throw new InvalidOperationException();
      }
    }

    private class DudErrorThrowerList : NamedEntityList<DudErrorThrower> {
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
        new TestEditor<DudErrorThrower, NamedBindingItem<DudErrorThrower>>(
          list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = "Dudley";
      Assert.Throws<InvalidOperationException>(() => list.OnRowValidated(0));
    }
  }
}