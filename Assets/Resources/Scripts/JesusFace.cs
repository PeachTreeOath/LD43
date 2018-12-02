﻿using System.Collections;
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
    private Color alphaColor;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        alphaColor = new Color(1, 1, 1, .25f);
    }

    void Update()
    {
        winceTime += Time.deltaTime;
        if (!isDoneWincing && winceTime > winceDuration)
        {
            GotoNeutralFace();
        }
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
}
