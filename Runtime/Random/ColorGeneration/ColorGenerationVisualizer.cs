using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ColorGenerationVisualizer : MonoBehaviour
{
    [Header("Color Settings")]
    public ColorGenerationSchema colorSchema;

    [Header("Layout Settings")]
    public int seed = 12345;
    public bool randomSeed = true;
    [Range(1, 100)] public int columns = 10;
    [Range(1, 100)] public int rows = 10;
    [Range(0f, 1f)] public float spacing = 0.1f;
    public Vector2 sizeRange = new Vector2(0.5f, 1.5f);

    private List<GameObject> colorObjects = new List<GameObject>();
    private System.Random random;
    private bool needsRegeneration = true;

    protected virtual void OnEnable()
    {
        GenerateColors();
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

#if UNITY_EDITOR
    private void EditorUpdate()
    {
        if (colorSchema != null && EditorUtility.IsDirty(colorSchema))
        {
            needsRegeneration = true;
        }
    }
#endif

    protected virtual void OnValidate()
    {
        needsRegeneration = true;
    }

    protected virtual void Update()
    {
        if (needsRegeneration)
        {
            GenerateColors();
            needsRegeneration = false;
        }
    }

    [ContextMenu("Regenerate Colors")]
    public void RegenerateColors()
    {
        needsRegeneration = true;
    }

    private void GenerateColors()
    {
        ClearExisting();

        if (colorSchema == null || colorSchema.items == null || colorSchema.items.Length == 0)
        {
            Debug.LogWarning("Cannot generate colors: schema is null or has no items");
            return;
        }

        if (randomSeed)
        {
            seed = Random.Range(0, int.MaxValue);
            randomSeed = false;
        }
        random = new System.Random(seed);

        float totalWidth = columns * (1 + spacing) - spacing;
        float totalHeight = rows * (1 + spacing) - spacing;
        Vector3 startPos = new Vector3(-totalWidth / 2f, totalHeight / 2f, 0);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Color color = ColorGenerationUtils.Generate(colorSchema, random, null);
                Vector3 pos = startPos + new Vector3(
                    x * (1 + spacing),
                    -y * (1 + spacing),
                    0);

                float size = Mathf.Lerp(sizeRange.x, sizeRange.y, (float)random.NextDouble());
                CreateColorQuad(pos, color, size);
            }
        }
    }

    private void CreateColorQuad(Vector3 position, Color color, float size)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = $"ColorQuad_{colorObjects.Count}";
        quad.transform.SetParent(transform);
        quad.transform.localPosition = position;
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = Vector3.one * size;

        Renderer renderer = quad.GetComponent<Renderer>();
        renderer.sharedMaterial = new Material(Shader.Find("Unlit/Color")) { color = color };

        colorObjects.Add(quad);
    }

    private void ClearExisting()
    {
        colorObjects.RemoveAll(obj => obj == null);

        foreach (var obj in colorObjects)
        {
            if (obj != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(obj);
                }
                else
                {
                    DestroyImmediate(obj);
                }
            }
        }

        colorObjects.Clear();

        while (transform.childCount > 0)
        {
            var child = transform.GetChild(0).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }
}