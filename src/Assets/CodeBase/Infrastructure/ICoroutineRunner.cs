using System.Collections;
using UnityEngine;

namespace Game.CodeBase.Infrastructure
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}