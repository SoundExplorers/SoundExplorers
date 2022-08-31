using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Model {
  [UsedImplicitly]
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [VelocityDb.Indexing.Index("_simpleKey")]
  public class ErrorThrower : EntityBase, INamedEntity {

    public ErrorThrower() : base(typeof(ErrorThrower), nameof(Name),
      null) { }

    [ExcludeFromCodeCoverage]
    public string Name {
      get => SimpleKey;
      set => throw new InvalidOperationException();
    }
  }
}