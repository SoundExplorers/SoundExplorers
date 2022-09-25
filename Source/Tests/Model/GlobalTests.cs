using System;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model; 

[TestFixture]
public class GlobalTests {
  private const string Message = "Test message";
  private const string Path = @"C:\My Folder\My Text File.txt";
  private const string PathDescription = "Flat file";

  private class ConstructorExceptionThrowingList : ActList {
    public ConstructorExceptionThrowingList() {
      throw new InvalidOperationException();
    }
  }

  [Test]
  public void CreateEntityList() {
    Assert.IsInstanceOf<ActList>(Global.CreateEntityList(typeof(ActList)), "Works");
    Assert.Throws<InvalidOperationException>(
      () => Global.CreateEntityList(typeof(ConstructorExceptionThrowingList)),
      "Throws");
  }

  [Test]
  public void CreateFileExceptionFromIoException() {
    var exception = Global.CreateFileException(new FileNotFoundException(Message),
      PathDescription, Path);
    Assert.IsInstanceOf<ApplicationException>(exception, "Exception type");
    Assert.IsTrue(
      exception.Message.Contains(PathDescription) &&
      exception.Message.Contains(Message), "Message");
  }

  [Test]
  public void CreateFileExceptionFromNotSupportedException() {
    var exception = Global.CreateFileException(new NotSupportedException(Message),
      PathDescription, Path);
    Assert.IsInstanceOf<ApplicationException>(exception, "Exception type");
    Assert.IsTrue(
      exception.Message.Contains(PathDescription) &&
      exception.Message.Contains(Path), "Message");
  }

  [Test]
  public void CreateFileExceptionFromOtherException() {
    var originalException = new InvalidOperationException(Message);
    var exception =
      Global.CreateFileException(originalException, PathDescription, Path);
    Assert.AreSame(exception, originalException);
  }
}