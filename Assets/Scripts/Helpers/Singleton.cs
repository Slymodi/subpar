using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// To access the heir by a static field "Instance".
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();

                GameObject parent = GameObject.Find("Systems");
                if (!parent)
                {
                    parent = new GameObject("Systems");
                    parent.AddComponent<DontDestroyOnLoad>();
                }
                go.transform.parent = parent.transform;

                instance = go.AddComponent<T>();
                go.name = instance.GetType().ToString();

                instance.AwakeSingleton();
            }
            return instance;
        }
    }

    protected virtual void AwakeSingleton() { }
}

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}