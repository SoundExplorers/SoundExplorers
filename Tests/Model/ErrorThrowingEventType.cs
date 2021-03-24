using System;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Model {
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  public class ErrorThrowingEventType : EventType {
    [ExcludeFromCodeCoverage]
    public new string Name {
      get => SimpleKey;
      set => throw new InvalidOperationException();
    }
  }
}