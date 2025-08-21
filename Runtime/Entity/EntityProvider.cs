using System;
using UnityEngine;

public class EntityProvider : MonoBehaviour, IEntity
{
    private Entity entity;

    public Entity Entity => entity;

    public Guid EntityId => Entity.Id;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        if (entity == null)
        {
            entity = GetComponentInParent<Entity>();
        }

        if (entity == null)
        {
            Debug.LogError(
                $"{nameof(EntityProvider)} assumes to be attached to an entity. " +
                $"Please, attach {nameof(Entity)} component to the " +
                "GameObject or to its parent.");
        }
    }
}
