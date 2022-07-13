using System;
using Mirror;
using Newtonsoft.Json;
using UnityEngine;

namespace CodeBase.Model
{
    [Serializable]
    public class ColorData
    {
        [JsonProperty] public float R { get; set; }
        [JsonProperty] public float G { get; set; }
        [JsonProperty] public float B { get; set; }

        [JsonIgnore] public readonly Color Color;

        [JsonConstructor]
        public ColorData(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            Color = new Color(R, G, B);
        }

        public static implicit operator ColorData(Color color) =>
            new(color.r, color.g, color.b);
    }

    public static class ColorDataReaderWriter
    {
        public static void WriteDateTime(this NetworkWriter writer, ColorData color)
        {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
        }

        public static ColorData ReadDateTime(this NetworkReader reader)
        {
            return new ColorData(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
        }
    }
}