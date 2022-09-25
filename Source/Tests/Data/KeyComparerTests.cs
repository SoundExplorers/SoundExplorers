using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data; 

[TestFixture]
public class KeyComparerTests {
  [SetUp]
  public void Setup() {
    KeyComparer = new KeyComparer();
  }

  private KeyComparer KeyComparer { get; set; } = null!;

  [Test]
  public void Equals() {
    var key1 = new Key("A", null);
    var key2 = new Key("A", null);
    Assert.AreEqual(0, KeyComparer.Compare(key1, key2));
  }

  [Test]
  public void GreaterThan() {
    var key1 = new Key("B", null);
    var key2 = new Key("A", null);
    Assert.AreEqual(1, KeyComparer.Compare(key1, key2));
  }

  [Test]
  public void LessThan() {
    var key1 = new Key("A", null);
    var key2 = new Key("B", null);
    Assert.AreEqual(-1, KeyComparer.Compare(key1, key2));
  }
}