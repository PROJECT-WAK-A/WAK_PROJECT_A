using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance = null;
    private static object _syncobj = new object();
    private static bool applsClosing = false;

    public static T Instance
    {
        get
        {
            if (applsClosing)
            {
                return null;
            }
            lock (_syncobj)
            {
                if (_instance == null)
                {
                    T[] objs = FindObjectsOfType<T>();

                    if (objs.Length > 0)
                    {
                        _instance = objs[0];
                    }
                    // 객체는 하나만 존재해야함
                    if (objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }

                    if (_instance == null)
                    {
                        string goName = typeof(T).ToString();
                        GameObject go = GameObject.Find(goName) ?? new GameObject(goName);
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void OnApplicationQuit()
    {
        applsClosing = true;
    }
}
