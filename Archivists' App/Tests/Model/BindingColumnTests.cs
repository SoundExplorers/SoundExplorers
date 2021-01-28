using System;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class BindingColumnTests {
    [Test]
    public void ValidateInstantiations() {
      const string name = "Newsletter";
      var referencedEntityListType = typeof(NewsletterList);
      const string referencedColumnName = "Date";
      BindingColumn dummy;
      Assert.DoesNotThrow(() => dummy = new BindingColumn(name, typeof(string)), "Name only");
      Assert.DoesNotThrow(
        () => dummy =
          new BindingColumn(name, typeof(string), referencedEntityListType, referencedColumnName),
        "All arguments specified and valid");
      Assert.Throws<ArgumentException>(() =>
          dummy = new BindingColumn(name, typeof(string), typeof(Newsletter), referencedColumnName),
        "Invalid referencedEntityListType");
      Assert.Throws<InvalidOperationException>(() =>
          dummy = new BindingColumn(name, typeof(string), referencedEntityListType),
        "Missing referencedColumnName.");
      Assert.Throws<InvalidOperationException>(() =>
          dummy = new BindingColumn(name, typeof(string), null, referencedColumnName),
        "Missing referencedEntityListType.");
    }
  }
}