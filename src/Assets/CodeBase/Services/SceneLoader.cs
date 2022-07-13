using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.CodeBase.Services
{
    public class SceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) => _coroutineRunner = coroutineRunner;

        public void Load(string name, Action onLoaded = null) => _coroutineRunner.StartCoroutine((LoadScene(name, onLoaded)));

        public async UniTask LoadAsync(string name)
        {
            await LoadSceneAsync(name);
        }


        private IEnumerator LoadScene(string nextScene, Action onLoaded = null) =>
            UniTask.ToCoroutine(() => LoadSceneAsync(nextScene, onLoaded));

        private static async UniTask LoadSceneAsync(string nextScene, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == nextScene)
            {
                onLoaded?.Invoke();
                await UniTask.CompletedTask;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            await UniTask.WaitUntil(() => waitNextScene.isDone);

            onLoaded?.Invoke();
        }
    }
}