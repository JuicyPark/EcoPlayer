using Evereal.YoutubeDLPlayer;
using FullSerializer;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ecoplayer
{
    public string user;
    public string youtubeLink;
    Ecoplayer(string user, string youtubeLink)
    {
        this.user = user;
        this.youtubeLink = youtubeLink;
    }
}

public class FirebaseLoader : MonoBehaviour
{
    public const string firebasePath = "https://ecoplayer-64d9d.firebaseio.com/ecoplayer.json";
    static fsSerializer serializer = new fsSerializer();
    [SerializeField] string version;

    [SerializeField] Text console;
    [SerializeField] Text erroConsole;

    [SerializeField] PathParser pathParser;
    [SerializeField] GameObject coreContainer;
    [SerializeField] VideoPlayerController videoPlayer;

    [SerializeField] Animator ecoPlayerAnimator;
    [SerializeField] Animator btnGroupAnimator;
    [SerializeField] Animator movieAnimator;

    string FindYoutubeLink(string allMessage)
    {
        char[] sep = { '\n', '\t', ' ' };
        string[] result = allMessage.Split(sep);
        foreach (var item in result)
        {
            if (item.Contains("https://www.youtube.com/watch") || item.Contains("https://youtu.be/"))
            {
                if (item.Contains(":ecoplayer:"))
                    return null;
                return item;
            }
        }
        return null;
    }

    void LoadFirebase()
    {
        RestClient.Get(firebasePath).Then(response =>
        {
            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, Ecoplayer>), ref deserialized);

            var users = deserialized as Dictionary<string, Ecoplayer>;

            foreach (var user in users)
            {
                var link = user.Value.youtubeLink;
                link = FindYoutubeLink(link);
                if (link != null)
                    ListContainer.Instance.links.Add(link);
            }
        });
    }

    void ReconnectParser()
    {
        erroConsole.text = "";
        console.text = "다시 접속합니다.";
        StopAllCoroutines();
        StartCoroutine(CParseYoutubeLink());
    }

    void InitAnimator()
    {
        ecoPlayerAnimator.enabled = true;
        btnGroupAnimator.enabled = true;
        movieAnimator.enabled = true;
    }

    void StartEcoPlayer()
    {
        console.text = "";
        InitAnimator();
        videoPlayer.PlayerStart();
    }

    IEnumerator CSpinWait()
    {
        while (ListContainer.Instance.links.Count != ListContainer.Instance.paths.Count)
        {
            if (ListContainer.Instance.paths.Count > 0)
                console.text = "<color='yellow'>링크</color>를 <color='green'>노동요</color>로 변환 중입니다.";
            else
            {
                if (Input.GetKeyDown(KeyCode.F5))
                    ReconnectParser();
            }
            yield return null;
        }
    }

    IEnumerator CParseYoutubeLink()
    {
        yield return new WaitForSeconds(1.5f);
        console.text = "<color='yellow'>링크</color>를 받아오고 있습니다.\n 너무 오래걸릴시 F5를 눌러주세요.";

        pathParser.ExecuteParse();
        yield return CSpinWait();

        console.text = "<color='blue'>ECOPLAYER</color> 를 실행합니다.";
        Destroy(coreContainer);
        yield return new WaitForSeconds(2f);
        StartEcoPlayer();
    }

    void Start()
    {
        console.text = "<color='blue'>ECOPLAYER</color>\nversion " + version;
        LoadFirebase();
        StartCoroutine(CParseYoutubeLink());
    }
}
