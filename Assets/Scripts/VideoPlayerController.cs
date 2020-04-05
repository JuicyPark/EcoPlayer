using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] GameObject farmObject;
    [SerializeField] GameObject catObject;
    [SerializeField] GameObject videoObject;

    [SerializeField] Animator farmAnimator;
    [SerializeField] Animator catAnimator;

    [SerializeField] Animator ecoPlayerAnimator;

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] EcoplayerUI pauseUI;

    bool isRecycle;
    bool isAccelate;
    bool isVideo;
    bool isStart;

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

    public void Accelate()
    {
        if (!isAccelate)
        {
            isAccelate = true;
            videoPlayer.playbackSpeed = 1.5f;
            catObject.SetActive(true);
        }
        else
        {
            isAccelate = false;
            videoPlayer.playbackSpeed = 1.0f;
            catObject.SetActive(false);
        }
    }

    public void PlayVideo()
    {
        if (!isVideo)
        {
            isVideo = true;
            farmObject.SetActive(false);
            videoObject.SetActive(true);
        }
        else
        {
            isVideo = false;
            farmObject.SetActive(true);
            videoObject.SetActive(false);
        }
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
        SetAnimator(true);
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
        SetAnimator(true);
    }

    public void Pause()
    {
        if (videoPlayer.isPlaying)
        {
            isStart = false;
            SetAnimator(false);
            videoPlayer.Pause();
        }
        else
        {
            SetAnimator(true);
            videoPlayer.Play();
        }
    }

    void SetAnimator(bool active)
    {
        catAnimator.enabled = active;
        farmAnimator.enabled = active;
        ecoPlayerAnimator.enabled = active;
    }

    public void PlayerStart()
    {
        Shuffle();
        videoPlayer.url = ListContainer.Instance.paths[playIndexs[currentIndex]];
        isStart = true;
    }

    public void UpSound() => volumeSlider.value += 0.1f;
    public void DownSound() => volumeSlider.value -= 0.1f;

}
