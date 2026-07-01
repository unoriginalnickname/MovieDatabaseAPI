/// <summary>Request used to assign a set of actors to a movie.</summary>
/// <example>
/// {
///   "actorIds": [1, 2, 3]
/// }
/// </example>
public class SetActorsDto
{
    /// <summary>List of actor IDs to assign.</summary>
    /// <example>[1, 2, 3]</example>
    public List<int> ActorIds { get; set; } = new();
}