using System;
using System.Data;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class EntityBaseTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
    }

    private class DudDaughter : Daughter {
      public DudDaughter([NotNull] QueryHelper queryHelper,
        Type identifyingParentType = null) :
        base(queryHelper, identifyingParentType) { }

      public override Mother Mother {
        get => IdentifyingParent as Mother;
        set {
          UpdateNonIndexField();
          IdentifyingParent = value;
        }
      }
    }

    private QueryHelper QueryHelper { get; set; }

    [Test]
    public void T010_DisallowInconsistentIdentifyingParent() {
      var dudDaughter1 = new DudDaughter(QueryHelper) {Name = "Xenia"};
      var dudDaughter2 = new DudDaughter(QueryHelper, typeof(Father))
        {Name = "Yvette"};
      var mother1 = new Mother(QueryHelper);
      Assert.Throws<ConstraintException>(
        () => dudDaughter1.Mother = mother1,
        "IdentifyingParentType has not been specified");
      Assert.Throws<ConstraintException>(
        () => dudDaughter2.Mother = mother1,
        "Value's type is not IdentifyingParentType");
    }
  }
}