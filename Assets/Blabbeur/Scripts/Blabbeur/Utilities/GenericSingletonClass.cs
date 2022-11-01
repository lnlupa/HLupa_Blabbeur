using UnityEngine;

/// <summary>
/// An abstract class which allows any class which inherits it to be treated as a singleton class
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericSingletonClass<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AfterAwake();
    }

    /// <summary>
    /// Overriding this function allows a child of this class to define their own additional Awake() functionality
    /// </summary>
    protected virtual void AfterAwake()
    {
    }
}