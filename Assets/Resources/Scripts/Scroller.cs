using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour {
    // Scroll main texture based on time

    public float scrollSpeed;
    Renderer rend;

    private float lastScrollSpeed;

    void Start()
    {
        rend = GetComponent<Renderer>();
        lastScrollSpeed = scrollSpeed;
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }

    public void pause() {
        Debug.Log("Scroller paused");
        lastScrollSpeed = scrollSpeed;
        scrollSpeed = 0;
    }

    public void resume() {
        Debug.Log("Scroller resumed");
        scrollSpeed = lastScrollSpeed;
    }
}
