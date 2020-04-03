using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Animator movieAnimator;
    [SerializeField] Animator ecoPlayerAnimator;
    [SerializeField] Slider volumeSlider;
    [SerializeField] EcoplayerUI pauseUI;
    bool isRecycle = false;
    bool isRainy = false;
    bool isStart = false;
    List<int> playIndexs = new List<int>();
    int currentIndex = 0;

    IEnumerator CTerm()
    {
        yield return new WaitForSeconds(5f);
        isStart = true;
    }

    void Update()
    {
        videoPlayer.SetDirectAudioVolume(0,volumeSlider.value);
        if(isStart)
        {
            if (videoPlayer.isPaused)
            {
                if (isRecycle)
                    Current();
                else
                    Next();
                isStart = false;
                StartCoroutine(CTerm());
            }
        }
    }

    void Swap(int aIndex, int bIndex)
    {
        int temp = playIndexs[aIndex];
        playIndexs[aIndex] = playIndexs[bIndex];
        playIndexs[bIndex] = temp;
    }

    void Shuffle()
    {
        int pathCount = ListContainer.Instance.paths.Count;
        for (int i = 0; i < pathCount; i++)
            playIndexs.Add(i);

        for (int i = 0; i < 100; i++)
        {
            int a = Random.Range(0, pathCount);
            int b = Random.Range(0, pathCount);
            Swap(a, b);
        }
    }

    public void Recycle()
    {
        if (!isRecycle)
            isRecycle = true;
        else
            isRecycle = false;
    }

    public void Rainy()
    {
        if (!isRainy)
            isRainy = true;
        else
            isRainy = false;
    }

    public void Current()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
        isStart = true;
    }

    public void Prev()
    {
        pauseUI.Init();
        videoPlayer.Stop();
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = playIndexs.Count - 1;
        videoPlayer.url = ListContainer.Instance.paths[playIndexs[currentIndex]];
        videoPlayer.Play();

        isStart = true;
        movieAnimator.enabled = true;
        ecoPlayerAnimator.enabled = true;
    }

    public void Next()
    {
        pauseUI.Init();
        videoPlayer.Stop();
        currentIndex++;
        if (currentIndex >= playIndexs.Count)
            currentIndex = 0;
        videoPlayer.url = ListContainer.Instance.paths[playIndexs[currentIndex]];
        videoPlayer.Play();

        isStart = true;
        movieAnimator.enabled = true;
        ecoPlayerAnimator.enabled = true;
    }

    public void Pause()
    {
        if (videoPlayer.isPlaying)
        {
            movieAnimator.enabled = false;
            ecoPlayerAnimator.enabled = false;
            isStart = false;
            videoPlayer.Pause();
        }
        else
        {
            movieAnimator.enabled = true;
            ecoPlayerAnimator.enabled = true;
            videoPlayer.Play();
        }
    }

    public void PlayerStart()
    {
        Shuffle();
        videoPlayer.url = ListContainer.Instance.paths[playIndexs[currentIndex]];
        isStart = true;
    }
}
