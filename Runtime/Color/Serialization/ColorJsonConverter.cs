using UnityEngine;
using Newtonsoft.Json;
using System;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        // Serialize color to hex-format (#RRGGBB)
        writer.WriteValue($"#{ColorUtility.ToHtmlStringRGB(value)}");
    }

    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string hex = reader.Value?.ToString();
        if (string.IsNullOrEmpty(hex))
        {
            return default;
        }

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        return Color.magenta;
    }
}
