using System;
using UnityEngine;
using System.Collections;
using UnityUtility.Singleton;
using UnityEngine.SceneManagement;

namespace UnityUtility.Manager
{
    public class LoadingSceneManager<TScene> : GenericSingleton<LoadingSceneManager<TScene>> where TScene : Enum
    {
        private TScene currentScene;
        private bool isLoading = false;

        public event EventHandler<TScene> OnSceneChange;

        public void LoadScene(TScene scene)
        {
            if (isLoading)
            {
                Debug.LogWarning("A scene is already loading.");
                return;
            }

            Instance.StartCoroutine(LoadSceneAsync(scene));
        }

        private IEnumerator LoadSceneAsync(TScene scene)
        {
            isLoading = true;

            AsyncOperation operation = SceneManager.LoadSceneAsync(Convert.ToInt32(scene));

            while (!operation.isDone)
                yield return null;

            currentScene = scene;
            isLoading = false;

            OnSceneChange?.Invoke(this, currentScene);
        }

        public TScene CurrentScene => currentScene;
    }

}