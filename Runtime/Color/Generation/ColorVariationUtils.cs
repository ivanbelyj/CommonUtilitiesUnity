using UnityEngine;
using System;

[Serializable]
public struct ColorVariationSettings
{
    [Tooltip("Enable hue variation")]
    public bool allowHueVariation;

    [Header("Lightness Variation")]
    [Tooltip("Maximum lightness decrease (0-1)")]
    [Range(0f, 1f)] public float maxLightnessDecrease;

    [Tooltip("Maximum lightness increase (0-1)")]
    [Range(0f, 1f)] public float maxLightnessIncrease;

    [Header("Hue Variation")]
    [Tooltip("Hue variation range in degrees (0-360)")]
    [Range(0f, 360f)] public float hueVariationRange;

    [Header("Saturation Variation")]
    [Tooltip("Saturation variation range (0-1)")]
    [Range(0f, 1f)] public float saturationVariationRange;

    [Header("Channel Variation")]
    [Tooltip("RGB channel variation range (0-1)")]
    [Range(0f, 1f)] public float channelVariationRange;

    [Header("Distribution")]
    [Tooltip("Use normal distribution for variations")]
    public bool useNormalDistribution;

    public static ColorVariationSettings Default => new()
    {
        allowHueVariation = true,
        maxLightnessDecrease = 0.05f,
        maxLightnessIncrease = 0.2f,
        hueVariationRange = 10f,
        saturationVariationRange = 0.2f,
        channelVariationRange = 0.1f,
        useNormalDistribution = true
    };
}

// Ai-generated
public static class ColorVariationUtils
{
    public static Color GenerateVariation(Color baseColor, in ColorVariationSettings settings, System.Random random = null)
    {
        random ??= new System.Random();
        float[] hsl = RGBToHSL(baseColor);
        Color result = baseColor;

        // Apply all variations in controlled sequence
        result = ApplyLightnessVariation(result, settings, random);
        if (settings.allowHueVariation)
        {
            result = ApplyHueVariation(result, settings, random);
        }
        result = ApplySaturationVariation(result, settings, random);
        result = ApplyChannelVariation(result, settings, random);

        return result;
    }

    #region Lightness variation
    private static Color ApplyLightnessVariation(Color color, in ColorVariationSettings settings, System.Random random)
    {
        if (settings.maxLightnessDecrease <= 0f && settings.maxLightnessIncrease <= 0f)
            return color;

        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);

        // We generate a variation in the full range [-decrease, +increase]
        float variation = GetVariationValue(
            -settings.maxLightnessDecrease,
            settings.maxLightnessIncrease,
            settings.useNormalDistribution,
            random);

        // Apply a variation based on the current brightness
        float newV = v + variation * GetLightnessVariationWeight(v, settings);

        // Soft restriction
        newV = SoftClamp(newV, 0.02f, 0.98f);

        // Saturation correction
        if (s > 0.5f)
        {
            s = Mathf.Clamp01(s * (1.1f - Mathf.Abs(variation)));
        }

        return Color.HSVToRGB(h, s, newV).WithAlpha(color.a);
    }

    private static float GetLightnessVariationWeight(float currentV, in ColorVariationSettings settings)
    {
        // Gradually reducing the effect of variation at the boundaries of the range
        float decreaseWeight = Mathf.Clamp01(currentV / 0.2f); // Full strength when v > 0.2
        float increaseWeight = Mathf.Clamp01((1f - currentV) / 0.2f); // Full strength when v < 0.8

        return Mathf.Lerp(decreaseWeight, increaseWeight,
               settings.maxLightnessIncrease / (settings.maxLightnessDecrease + settings.maxLightnessIncrease));
    }

    private static float SoftClamp(float value, float min, float max)
    {
        // A soft constraint with parabolic edges
        if (value < min)
        {
            float d = min - value;
            return min - d * d * 0.1f;
        }
        if (value > max)
        {
            float d = value - max;
            return max + d * d * 0.1f;
        }
        return value;
    }
    #endregion

    private static Color ApplyHueVariation(Color color, in ColorVariationSettings settings, System.Random random)
    {
        if (settings.hueVariationRange <= 0.001f || !settings.allowHueVariation)
            return color;

        // Getting the current HSL components
        float h, s, l;
        Color.RGBToHSV(color, out h, out s, out l);

        // Generating a shade variation
        float variation = GetVariationValue(
            -settings.hueVariationRange,
            settings.hueVariationRange,
            settings.useNormalDistribution,
            random) / 360f;

        // Apply variation
        h = (h + variation + 1f) % 1f;

        // Convert back to RGB, keeping the original saturation and value
        return Color.HSVToRGB(h, s, l).WithAlpha(color.a);
    }

    private static Color ApplySaturationVariation(Color color, in ColorVariationSettings settings, System.Random random)
    {
        if (settings.saturationVariationRange <= 0.001f)
            return color;

        float[] hsl = RGBToHSL(color);
        float baseSaturation = hsl[1];

        // Variation as a percentage of the current saturation
        float variation = GetVariationValue(
            -settings.saturationVariationRange,
            settings.saturationVariationRange,
            settings.useNormalDistribution,
            random);

        // For low saturation, we use the absolute change
        // For high - relative
        float newSaturation;
        if (baseSaturation < 0.3f)
        {
            newSaturation = baseSaturation + variation * 0.5f;
        }
        else
        {
            newSaturation = baseSaturation * (1 + variation);
        }

        hsl[1] = Mathf.Clamp01(newSaturation);
        return HSLToRGB(hsl[0], hsl[1], hsl[2], color.a);
    }

    private static Color ApplyChannelVariation(Color color, in ColorVariationSettings settings, System.Random random)
    {
        if (settings.channelVariationRange <= 0.001f)
            return color;

        float variation = GetVariationValue(
            -settings.channelVariationRange,
            settings.channelVariationRange,
            settings.useNormalDistribution,
            random);

        // Save relative proportions of channels
        float r = color.r * (1 + variation * 0.5f);
        float g = color.g * (1 + variation * 0.7f);
        float b = color.b * (1 + variation * 0.3f);

        // Normalize to save brightness
        float max = Mathf.Max(r, g, b);
        if (max > 1f)
        {
            r /= max;
            g /= max;
            b /= max;
        }

        return new Color(
            Mathf.Clamp01(r),
            Mathf.Clamp01(g),
            Mathf.Clamp01(b),
            color.a);
    }

    private static float GetVariationValue(float min, float max, bool useNormalDistribution, System.Random random)
    {
        if (useNormalDistribution)
        {
            // Box-Muller transform for normal distribution
            float u1 = 1.0f - (float)random.NextDouble();
            float u2 = 1.0f - (float)random.NextDouble();
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

            // Scale to desired range (3 sigma rule)
            float randNormal = min + (max - min) * (randStdNormal + 3f) / 6f;
            return Mathf.Clamp(randNormal, min, max);
        }

        return min + (float)random.NextDouble() * (max - min);
    }

    // private static float GetVariationValue(float min, float max, bool useNormalDistribution, System.Random random)
    // {
    //     if (useNormalDistribution)
    //     {
    //         float u1 = 1f - (float)random.NextDouble();
    //         float u2 = 1f - (float)random.NextDouble();
    //         float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
    //         randStdNormal = Mathf.Clamp(randStdNormal, -2.5f, 2.5f);
    //         return min + (max - min) * (randStdNormal + 2.5f) / 5f;
    //     }

    //     float t = (float)random.NextDouble();
    //     t = t * t * (3f - 2f * t);
    //     return min + (max - min) * t;
    // }

    private static float[] RGBToHSL(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));
        float min = Mathf.Min(r, Mathf.Min(g, b));
        float h, s, l = (max + min) / 2f;

        if (Mathf.Approximately(max, min))
        {
            h = s = 0f;
        }
        else
        {
            float d = max - min;
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (Mathf.Approximately(max, r))
                h = (g - b) / d + (g < b ? 6f : 0f);
            else if (Mathf.Approximately(max, g))
                h = (b - r) / d + 2f;
            else
                h = (r - g) / d + 4f;

            h /= 6f;
            if (h < 0f) h += 1f;
        }

        return new float[] { h, s, l };
    }

    private static Color HSLToRGB(float h, float s, float l, float alpha = 1f)
    {
        h = Mathf.Repeat(h, 1f);
        s = Mathf.Clamp01(s);
        l = Mathf.Clamp01(l);

        if (s < 0.001f)
        {
            return new Color(l, l, l, alpha);
        }

        float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
        float p = 2f * l - q;

        float r = HueToRGB(p, q, h + 1f / 3f);
        float g = HueToRGB(p, q, h);
        float b = HueToRGB(p, q, h - 1f / 3f);

        return new Color(r, g, b, alpha);
    }

    private static float HueToRGB(float p, float q, float t)
    {
        if (t < 0f) t += 1f;
        if (t > 1f) t -= 1f;

        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }
}