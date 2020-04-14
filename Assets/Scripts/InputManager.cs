using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] VideoPlayerController videoPlayerController;
    [SerializeField] EcoplayerUI[] ecoplayerUIs;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            videoPlayerController.Next();
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            videoPlayerController.Prev();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayerController.Pause();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            videoPlayerController.UpSound();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            videoPlayerController.DownSound();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ecoplayerUIs[0].PressToggle();
            videoPlayerController.Recycle();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ecoplayerUIs[1].PressToggle();
            videoPlayerController.Accelate();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ecoplayerUIs[2].PressToggle();
            videoPlayerController.PlayVideo();
        }
    }
}
