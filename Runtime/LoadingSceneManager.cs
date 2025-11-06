using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum Scene
{
    None,
}

public class LoadingSceneManager : GenericSingleton<LoadingSceneManager>
{
    [Header("Scene Management")]
    [SerializeField] private Scene currentScene;
    [SerializeField] private bool isLoading = false;

    public event EventHandler<Scene> OnSceneChange;

    /// <summary>
    /// Starts loading the given scene asynchronously.
    /// If a scene is already loading, the method exits early.
    /// </summary>
    public void LoadScene(Scene scene)
    {
        if (isLoading)
        {
            Debug.LogWarning("A scene is already loading.");
            return;
        }

        StartCoroutine(LoadSceneAsync(scene));
    }

    /// <summary>
    /// Coroutine responsible for loading a new scene asynchronously.
    /// Updates internal state and notifies listeners upon completion.
    /// </summary>
    private IEnumerator LoadSceneAsync(Scene scene)
    {
        isLoading = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene);

        while (!operation.isDone)
            yield return null;

        currentScene = scene;
        isLoading = false;

        OnSceneChange?.Invoke(this, currentScene);
    }
}
