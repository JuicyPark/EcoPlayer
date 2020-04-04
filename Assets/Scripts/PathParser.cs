using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Evereal.YoutubeDLPlayer
{
    public class PathParser : MonoBehaviour
    {
        [SerializeField] Transform coreContainer;
        [SerializeField] Text erroText;
        public YTDLCore ytdlCore;
        private VideoInfo videoInfo;
        string errorMsg = null;
        string LOG_FORMAT = "[CoreSample] {0}";
        public List<VideoFormat> availableVideoFormat { get; private set; }

        void ParseCompleted(VideoInfo info)
        {
            videoInfo = info;
            ListContainer.Instance.paths.Add(Utils.ValidParsedVideoUrl(videoInfo, videoInfo.url));
        }

        void ErrorReceived(YTDLCore.ErrorEvent error)
        {
            errorMsg = error.message;
            erroText.text = "erro : " + errorMsg + " <color='yellow'>Juicy</color>에게 문의를...";
        }

        void ParseToPath(string link)
        {
            YTDLCore tempCore = Instantiate(ytdlCore, coreContainer).GetComponent<YTDLCore>();
            tempCore.parseCompleted += ParseCompleted;
            tempCore.errorReceived += ErrorReceived;
            StartCoroutine(tempCore.PrepareAndParse(link));
        }

        public void ExecuteParse()
        {
            foreach (var link in ListContainer.Instance.links)
                ParseToPath(link);
        }
    }
}