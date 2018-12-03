using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptionTimer : MonoBehaviour
{

    // Hooked in inspector
    public Image timerImage;

    public void SetImageRatio(float ratio)
    {
        timerImage.fillAmount = 1 - ratio;
    }
}
