using System;
using UnityEngine;

/// <summary>
/// Components or classes related to an entity
/// </summary>
public interface IEntity
{
    public Guid EntityId { get; }
}
