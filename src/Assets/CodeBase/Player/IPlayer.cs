using UniRx;
using UnityEngine;

namespace Game.CodeBase.Player
{
    public interface IPlayer
    {
        public int Id { get; }
        public string Username { get; }
        public Color Color { get; }

        ReactiveCommand<int> IdChanged { get; }
        ReactiveCommand<string> UsernameChanged { get; }
        ReactiveCommand<Color> ColorChanged { get; }

        void SetUsername(string username);
        void SetColor(Color value);
    }
}