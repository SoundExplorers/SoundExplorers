using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Model {
  [UsedImplicitly]
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  public class ErrorThrower : EntityBase, INamedEntity {
    public ErrorThrower() : base(typeof(ErrorThrower), nameof(Name),
      null) { }

    [ExcludeFromCodeCoverage]
    public string Name {
      get => SimpleKey;
      set => throw new InvalidOperationException();
    }

    [ExcludeFromCodeCoverage]
    protected override ISortedEntityCollection Root => ErrorThrowerRoot;

    [ExcludeFromCodeCoverage]
    private static SortedEntityCollection<ErrorThrower> ErrorThrowerRoot { get; set; } =
      null!;

    internal static void SetRoot(SortedEntityCollection<ErrorThrower> root) {
      ErrorThrowerRoot = root;
    }
  }
}