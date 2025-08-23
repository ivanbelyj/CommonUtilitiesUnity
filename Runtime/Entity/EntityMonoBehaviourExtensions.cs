using UnityEngine;

public static class EntityMonoBehaviourExtensions
{
    public static Entity GetEntity(this MonoBehaviour component)
    {
        if (!component.TryGetComponent<Entity>(out var entity))
        {
            entity = component.GetComponentInParent<Entity>();
        }

        if (entity == null)
        {
            Debug.LogError(
                $"{nameof(GetEntity)} method is assumed to be used " +
                "on an entity. " +
                $"Please, attach {nameof(Entity)} component to the " +
                "GameObject or to its parent.");
        }
        return entity;
    }
}
