using UnityEngine;

/// <summary>
/// A generic singleton base class for MonoBehaviour components.
/// </summary>
public class GenericSingleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    public static bool HasInstance => instance != null;

    /// <summary>
    /// Attempts to get the singleton instance.
    /// Returns null if the instance does not exist.
    /// </summary>
    /// <returns>The singleton instance or null.</returns>
    public static T TryGetInstance() => HasInstance ? instance : null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    var go = new GameObject(typeof(T).Name + " Generated");
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        InitializeSingleton();
    }


    /// <summary>
    /// Initializes the singleton instance by assigning <see cref="instance"/> to this component.
    /// Does nothing if not playing in runtime.
    /// </summary>
    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;

        instance = this as T;
    }
}
