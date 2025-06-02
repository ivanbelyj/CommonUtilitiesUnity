using System.Linq;
using UnityEngine;

public static class ColorGenerationUtils
{
    public static Color Generate(
        ColorGenerationSchema schema,
        System.Random random,
        Color? color)
    {
        var randomGenerationItem = RandomUtils.GetRandomWeighted(
            schema.items,
            schema.items.Select(x => x.weight).ToArray(),
            random);
        return ColorVariationUtils.GenerateVariation(
            color == null
                ? randomGenerationItem.colors.IsNullOrEmpty()
                    ? schema.defaultColor
                    : randomGenerationItem.colors.GetRandomOne(random)
                : color.Value,
            in randomGenerationItem.colorVariationSettings,
            random);
    }
}
