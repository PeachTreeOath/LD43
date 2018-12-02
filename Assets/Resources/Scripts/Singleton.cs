﻿using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;
    private bool doNotDestroy = false;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            gameObject.tag = "singleton";
            Debug.Log("Tagged " + gameObject.name + " as singleton");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void SetDontDestroy()
    {
        DontDestroyOnLoad(gameObject);
        doNotDestroy = true;
    }

    private void OnDestroy() {
        if(!doNotDestroy) 
            instance = null;
    }
}
