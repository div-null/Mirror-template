using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiniGameFabric
{
    public GameObject GetPlayerPrefab();
    public void InitializeMiniGame();
    public GameObject SpawnObject(string gameObjectPath);
}
