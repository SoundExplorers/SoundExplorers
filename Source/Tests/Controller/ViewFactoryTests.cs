using System;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller; 

[TestFixture]
public class ViewFactoryTests {
  [UsedImplicitly]
  private class ConstructorExceptionThrowingController {
    public ConstructorExceptionThrowingController(
      IView<ConstructorExceptionThrowingController> view) {
      throw new InvalidOperationException();
    }
  }

  [Test]
  public void ControllerConstructorException() {
    Assert.Throws<InvalidOperationException>(() => ViewFactory
      .Create<MockView<ConstructorExceptionThrowingController>,
        ConstructorExceptionThrowingController>());
  }
}