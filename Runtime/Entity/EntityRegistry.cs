using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityRegistry : MonoBehaviour
{
    private readonly Dictionary<Guid, Entity> entities = new();

    public IReadOnlyDictionary<Guid, Entity> Entities => entities;

    private static EntityRegistry instance;
    public static EntityRegistry Instance
    {
        get
        {
            if (instance is null)
            {
                instance = FindAnyObjectByType<EntityRegistry>();
                if (instance is null)
                {
                    Debug.LogError($"{nameof(EntityRegistry)} was not instantiated.");
                }
            }
            return instance;
        }
    }

    public void Register(Entity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (entities.ContainsKey(entity.Id))
        {
            Debug.LogError($"Entity with already registered (id: {entity.Id})");
            return;
        }

        entities.Add(entity.Id, entity);
    }

    public void Unregister(IEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        entities.Remove(entity.EntityId);
    }

    public bool TryGetEntity(Guid id, out Entity entity)
    {
        return entities.TryGetValue(id, out entity);
    }

    public void Clear()
    {
        entities.Clear();
    }
}