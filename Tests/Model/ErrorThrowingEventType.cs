using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Model {
  [UsedImplicitly]
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  public class ErrorThrowingEventType : EventType {
    public ErrorThrowingEventType(SortedEntityCollection<EventType> root) : base(
      root) { }

    [ExcludeFromCodeCoverage]
    public new string Name {
      get => SimpleKey;
      set => throw new InvalidOperationException();
    }
  }
}