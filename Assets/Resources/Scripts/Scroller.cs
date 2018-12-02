using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    // Scroll main texture based on time
    [HideInInspector]
    public float scrollSpeed;
    Renderer rend;

    private float lastScrollSpeed;
    Vector2 startOffset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        //scrollSpeed = GameManager.instance.roadSpeed;
        lastScrollSpeed = scrollSpeed;
        startOffset = rend.material.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(startOffset.x, offset));
    }

    public void pause()
    {
        Debug.Log("Scroller paused");
        lastScrollSpeed = scrollSpeed;
        scrollSpeed = 0;
    }

    public void resume()
    {
        Debug.Log("Scroller resumed");
        scrollSpeed = lastScrollSpeed;
    }
}
