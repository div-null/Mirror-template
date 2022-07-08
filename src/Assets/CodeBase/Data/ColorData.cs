using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.CodeBase.Data
{
    [Serializable]
    public class ColorData
    {
        [JsonRequired] public readonly float R;
        [JsonRequired] public readonly float G;
        [JsonRequired] public readonly float B;

        [JsonIgnore] public readonly Color Color;

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
}