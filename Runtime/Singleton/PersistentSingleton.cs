using UnityEngine;

/// <summary>
/// A generic singleton base class for MonoBehaviour components that persist across scenes.
/// Ensures only one instance of <typeparamref name="T"/> exists and is not destroyed when loading new scenes.
/// Optionally unparents the GameObject on Awake to avoid being a child of any other transform.
/// </summary>
public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    [Header("PersistentSingleton Settings")]
    public bool autoUnparentOnAwake = true;

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
    /// Initializes the singleton instance by assigning <see cref="instance"/> to this component if none exists.
    /// Calls <see cref="DontDestroyOnLoad(GameObject)"/> to keep the singleton alive across scene loads.
    /// Destroys the GameObject if another instance already exists.
    /// Optionally detaches the GameObject from its parent if <see cref="autoUnparentOnAwake"/> is true.
    /// </summary>
    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;

        if (autoUnparentOnAwake)
            transform.SetParent(null);

        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }

    }
}

