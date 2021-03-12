using System.Collections;
using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Base class for a default comparer to be used when sorting a parent entity list
  ///   containing entities with identifying parents. It currently works with Sets and
  ///   Pieces but not Events.
  /// </summary>
  /// <remarks>
  ///   This class would not be suitable for determining the order of SortedChildLists
  ///   (via EntityBase.Key). I tried. The problem was that it could break if used while
  ///   changing the identifying parent. (See PieceTests.ChangeSet.)
  /// </remarks>
  public abstract class DefaultIdentifiedEntityComparer<TEntity, TIdentifyingParent>
    : Comparer<TEntity>
    where TEntity : EntityBase
    where TIdentifyingParent : EntityBase {
    protected DefaultIdentifiedEntityComparer(IComparer identifyingParentComparer) {
      IdentifyingParentComparer = identifyingParentComparer;
    }

    private IComparer IdentifyingParentComparer { get; }

    public override int Compare(TEntity? entity1, TEntity? entity2) {
      // Debug.WriteLine("DefaultIdentifiedEntityComparer.Compare");
      // Now we can assume that neither entity is null.
      var identifyingParent1 = (entity1!.IdentifyingParent as TIdentifyingParent)!;
      var identifyingParent2 = (entity2!.IdentifyingParent as TIdentifyingParent)!;
      // Compare IdentifyingParents first.
      // Debug.WriteLine(
      //   $"    Comparing {entity1!.EntityType.Name}s '{entity1!.Key}' and " + 
      //   $"'{entity2!.Key}'");
      var identifyingParentComparer = IdentifyingParentComparer;
      int identifyingParentComparison =
        identifyingParentComparer.Compare(identifyingParent1, identifyingParent2);
      // Debug.WriteLine(
      //   $"    Comparison of {identifyingParent1.EntityType.Name}s of " + 
      //   $"{entity1!.EntityType.Name}s '{entity1!.Key}' and '{entity2!.Key}'\r\n" +
      //   $"        = {identifyingParentComparison}");
      if (identifyingParentComparison != 0) {
        return identifyingParentComparison;
      }
      // Same IdentifyingParent. Compare SimpleKeys. 
      int simpleKeyComparison =
        Key.CompareSimpleKeys(entity1.SimpleKey, entity2.SimpleKey);
      // Debug.WriteLine(
      //   $"    Comparison of SimpleKeys of " + 
      //   $"{entity1!.EntityType.Name}s '{entity1!.Key}' and '{entity2!.Key}'\r\n" + 
      //   $"        = {simpleKeyComparison}");
      return simpleKeyComparison;
    }
  }
}