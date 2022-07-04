using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;

//object in the NetworkManager that can be called when a minigame scene needs to be loaded
public abstract class IMiniGameLoader
{
    //Loading scene with tutorial
    public string LoadingScene;
    //Actual game scene
    public string GameScene;

    //Loading scenes for a specific mini-game
    public abstract void Load();
}

public interface MiniGameLoop
{
    //The event that is called when the mini game ends
    //List of places and points received by players
    //public Action<Places> MiniGameEnd;
    //Spawn players and other interactive objects for minigame
    public void Spawn();
}




