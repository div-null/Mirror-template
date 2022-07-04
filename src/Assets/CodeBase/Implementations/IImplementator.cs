using System.Collections;
using System.Collections.Generic;
using CodeBase;
using UnityEngine;

public interface IImplementator
{
    public GameObject LobbyPlayerPrefab { get; }
    public string Scene { get; }

    public void Setup();
    //Разное представление игрока (в лобби оно одно, в минииграх совсем другое,
    //но что их объединяет, так это идентификаторы для игроков. нужно знать слоты, в которых распаоложен игрок)
    public void Start();
}
