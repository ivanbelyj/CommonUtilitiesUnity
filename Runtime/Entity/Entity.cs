using System;
using UnityEngine;

public class Entity : MonoBehaviour, IEntity
{
    public Guid Id { get; set; }

    Guid IEntity.EntityId => Id;

    private void Awake()
    {
        // New entities each game for now, no loading
        if (Id == default)
        {
            Id = Guid.NewGuid();
        }

        EntityRegistry.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (EntityRegistry.Instance != null)
        {
            EntityRegistry.Instance.Unregister(this);
        }
    }
}
