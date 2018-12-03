using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JesusFace : MonoBehaviour
{
    public Sprite neutralFace;
    public Sprite winceFace;
    public float winceDuration;
    private float winceTime;
    private bool isDoneWincing;
    private SpriteRenderer spr;
    private bool isTriggered;
    private Color alphaColor;

    private JesusManager jesusManager;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(1f, 1f, 1f, 1f);
        isTriggered = false;

        jesusManager = GetComponentInParent<JesusManager>();
    }

    void Update()
    {
        winceTime += Time.deltaTime;
        if (!isDoneWincing && winceTime > winceDuration)
        {
            GotoNeutralFace();
        }

        bool isTransparent = isTriggered || GameManager.instance.isPrayerDeath();
        if (isTransparent)
        {
            spr.color = new Color(1f, 1f, 1f, 0.1f);
        }
        else 
        {
            spr.color = new Color(1f, 1f, 1f, 1f);
        }
        jesusManager.SetBodyTransparency(isTransparent);
    }

    // Should only be called internally via timer but leaving this public anyways
    public void GotoNeutralFace()
    {
        spr.sprite = neutralFace;
        isDoneWincing = true;
    }

    public void GotoWinceFace()
    {
        spr.sprite = winceFace;
        winceTime = 0;
        isDoneWincing = false;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        isTriggered = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isTriggered = false;
    }
}
