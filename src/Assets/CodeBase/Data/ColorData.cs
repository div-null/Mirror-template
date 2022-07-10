using System;
using Mirror;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.CodeBase.Data
{
    [Serializable]
    public class ColorData
    {
        [JsonRequired] public readonly float R;
        [JsonRequired] public readonly float G;
        [JsonRequired] public readonly float B;

        [JsonIgnore, DoNotSerialize] public readonly Color Color;

        [JsonConstructor]
        public ColorData(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            Color = new Color(R, G, B);
        }

        public static implicit operator Color(ColorData color) =>
            new Color(color.R, color.G, color.B);

        public static implicit operator ColorData(Color color) =>
            new ColorData(color.r, color.g, color.b);
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