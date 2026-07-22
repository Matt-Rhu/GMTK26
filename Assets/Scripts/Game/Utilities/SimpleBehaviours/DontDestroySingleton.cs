using UnityEngine;

public class DontDestroySingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T instance
    {
        get
        {
            _instance ??= SetupInstance();
            return _instance;
        }
    }

    private static T SetupInstance()
    {
        T inst = FindObjectOfType<T>();
        inst ??= new GameObject($"-{nameof(T)}").AddComponent<T>();
        return inst;
    }

    protected virtual void Awake()
    {
        if (_instance != null && FindObjectsOfType<T>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
