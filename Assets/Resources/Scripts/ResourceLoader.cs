using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : Singleton<ResourceLoader>
{
    //[HideInInspector]
    //public Sprite timeIcon;

    protected override void Awake()
    {
        base.Awake();
        LoadResources();
    }

    private void LoadResources()
    {
    //timeIcon = Resources.Load<Sprite>("Textures/icons_v2/white_time");
    }
}
