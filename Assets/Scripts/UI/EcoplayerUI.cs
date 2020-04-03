using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EcoplayerUI : MonoBehaviour
{
    [SerializeField] Image currentImage;
    [SerializeField] Sprite typeA;
    [SerializeField] Sprite typeB;
    public bool isType = false;

    public void Init()
    {
        isType = false;
        currentImage.sprite = typeA;
    }

    public void PressButton()
    {
        if (isType)
        {
            currentImage.sprite = typeA;
            isType = false;
        }
        else
        {
            currentImage.sprite = typeB;
            isType = true;
        }
    }
}
