using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
    public class PathParser : MonoBehaviour
    {
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
        }

        void ParseToPath(string link)
        {
            YTDLCore tempCore = Instantiate(ytdlCore).GetComponent<YTDLCore>();
            tempCore.parseCompleted += ParseCompleted;
            tempCore.errorReceived += ErrorReceived;
            StartCoroutine(tempCore.PrepareAndParse(link));
        }

        public void ExecuteParese()
        {
            foreach (var link in ListContainer.Instance.links)
            {
                ListContainer.Instance.names.Add(link.Key);
                ParseToPath(link.Value);
            }
        }
    }
}