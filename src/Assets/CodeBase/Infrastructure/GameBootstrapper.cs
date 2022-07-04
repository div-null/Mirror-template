using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.CodeBase.Infrastructure.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
{
    [SerializeField] private AdvancedNetworkManager _advancedNetworkManagerPrefab;
    public Game Game;
    
    private void Awake()
    {
        Game = new Game(this, Instantiate(_advancedNetworkManagerPrefab));
        Game.GameStateMachine.Enter<BootstrapState>();
    }
}
