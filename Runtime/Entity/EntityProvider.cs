using UnityEngine;

public class EntityProvider : MonoBehaviour
{
    private Entity entity;

    public Entity Entity => entity;

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
