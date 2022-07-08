using System;
using UnityEngine;
using Random = System.Random;

namespace Game.CodeBase.Data
{
    [Serializable]
    public class PlayerProgress
    {
        private static Random _random = new Random();

        public readonly string Username;
        public readonly ColorData ColorData;

        public PlayerProgress(string username, Color color)
        {
            Username = username;
            ColorData = color;
        }

        public static PlayerProgress Generate()
        {
            var username = "Player" + _random.Next(1000, 10000);
            var color = RandomColor();
            return new(username, color);
        }

        private static Color RandomColor()
        {
            int colorId = _random.Next(0, 3);
            return colorId switch
            {
                1 => Color.red,
                2 => Color.blue,
                3 => Color.green,
                _ => Color.yellow
            };
        }
    }
}