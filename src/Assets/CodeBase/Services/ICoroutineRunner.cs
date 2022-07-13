using System.Collections;
using UnityEngine;

namespace Game.CodeBase.Services
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}