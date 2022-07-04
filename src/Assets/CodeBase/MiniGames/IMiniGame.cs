using System;
using UnityEngine;

namespace CodeBase.MiniGame
{
    public interface IMiniGame
    {
        //Объекты, реализаующие интерфейсы, будут подаваться NetworkManager'у и замещать его деятельность
        //Реализация паттерна стратегии. Объект будет знать всё об миниигре и будет ей управлять.
        //Он знает на какой сцене происходит действие. Знает логику игры. Знает префабы, которые нужно заспавнить и когда засправнить.
        //Также он знает места, на которых должно что-то появиться. Он может полностью управлять игрой и изредка использовать NetworkManager чтобы что-то сделать.
        //Кроме этого он может воспользоваться своей фабрикой, которая также будет использовать NetworkManager тогда, когда будет нужно

        public event Action Started;
        public event Action Finished;
        
        public string GetScene();
        public int[] GetScores();

        public void StartMinigame();
    }
}