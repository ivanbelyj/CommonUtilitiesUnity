using UnityEngine;

[System.Serializable]
public record ColorGenerationSchemaItem
{
    public float weight = 1f;
    public Color[] colors;
    public ColorVariationSettings colorVariationSettings;
}

[CreateAssetMenu(
    fileName = "New Appearance Color Generation Schema",
    menuName = "Customizable Appearance 2D/Appearance Color Generation Schema",
    order = 52)]
[System.Serializable]
public class ColorGenerationSchema : ScriptableObject
{
    public string schemaName = "default";
    public Color defaultColor = Color.white;

    public ColorGenerationSchemaItem[] items;
}
