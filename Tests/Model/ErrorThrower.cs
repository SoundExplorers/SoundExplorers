using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Model {
  [UsedImplicitly]
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [VelocityDb.Indexing.Index("_name")]
  public class ErrorThrower : EntityBase, INamedEntity {
    private readonly string _name = null!;

    public ErrorThrower() : base(typeof(ErrorThrower), nameof(Name),
      null) { }

    [ExcludeFromCodeCoverage]
    public string Name {
      get => _name;
      set => throw new InvalidOperationException();
    }
  }
}