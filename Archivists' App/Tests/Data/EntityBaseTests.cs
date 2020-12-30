using System;
using System.Data;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Data;
using PropertyConstraintException = SoundExplorers.Data.PropertyConstraintException;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class EntityBaseTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
    }

    private class DudDaughter : Daughter {
      public DudDaughter([NotNull] QueryHelper queryHelper,
        Type? identifyingParentType = null) :
        base(queryHelper, identifyingParentType) { }

      public override Mother Mother {
        get => (IdentifyingParent as Mother)!;
        set {
          UpdateNonIndexField();
          IdentifyingParent = value;
        }
      }
    }

    private QueryHelper QueryHelper { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      var entity = new Series();
      Assert.IsFalse(entity.AllowOtherTypesOnSamePage, "AllowOtherTypesOnSamePage");
    }

    [Test]
    public void DisallowInconsistentIdentifyingParent() {
      const string motherName = "Winifred";
      var xenia = new DudDaughter(QueryHelper) {Name = "Xenia"};
      var yvette = new DudDaughter(QueryHelper, typeof(Father))
        {Name = "Yvette"};
      var zoe = new DudDaughter(QueryHelper, typeof(Mother))
        {Name = "Zoe"};
      var mother = new Mother(QueryHelper) {Name = motherName};
      Assert.Throws<ConstraintException>(
        () => xenia.Mother = mother,
        "IdentifyingParentType has not been specified");
      Assert.Throws<PropertyConstraintException>(
        () => yvette.Mother = mother,
        "Value's type is not IdentifyingParentType");
      zoe.Mother = mother;
      Assert.AreEqual(motherName, zoe.Mother.Name, "zoe.Mother.Name");
    }
  }
}