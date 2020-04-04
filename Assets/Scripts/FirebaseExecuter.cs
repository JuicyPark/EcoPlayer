using Evereal.YoutubeDLPlayer;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Link
{
    public string youtubeLink;
    public string user;

    public Link(string youtubeLink, string user)
    {
        this.youtubeLink = youtubeLink;
        this.user = user;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["youtubeLink"] = youtubeLink;
        result["user"] = user;
        return result;
    }
}

public class FirebaseExecuter : MonoBehaviour
{
    [SerializeField] Text console;
    [SerializeField] Text erroConsole;

    [SerializeField] Animator ecoPlayerAnimator;
    [SerializeField] Animator btnGroupAnimator;
    [SerializeField] Animator movieAnimator;

    [SerializeField] Transform coreContainer;

    [SerializeField] VideoPlayerController videoPlayer;
    [SerializeField] PathParser pathParser;
    bool reset = true;

    public const string firebasePath = "https://ecoplayer-64d9d.firebaseio.com/";
    public DatabaseReference reference;
    string FindLink(string allMessage)
    {
        char[] sep = { '\n', '\t', ' '};
        string[] result = allMessage.Split(sep);
        foreach (var item in result)
        {
            if (item.Contains("https://www.youtube.com/watch")|| item.Contains("https://youtu.be/"))
            {
                if (item.Contains(":ecoplayer:"))
                    return null;
                return item;
            }
        }
        return null;
    }

    void GetFirebaseLink()
    {
        FirebaseDatabase.DefaultInstance.GetReference("ecoplayer").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.Log("failed");
            else if (task.IsCompleted)
            {
                Firebase.Database.DataSnapshot snapshot = task.Result;
                foreach (var childSnapshot in snapshot.Children)
                {
                    var link = childSnapshot.Child("youtubeLink").Value.ToString();
                    link = FindLink(link);
                    if (link == null)
                        reference.Child("ecoplayer").Child(childSnapshot.Key.ToString()).RemoveValueAsync();
                    else
                        ListContainer.Instance.links.Add(link);
                }
            }
        });
    }

    void Awake()
    {
        Screen.SetResolution(450, 337, false);
    }

    void Start()
    {
        console.text = "<color='yellow'>링크</color>를 받아오고 있습니다.";
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(firebasePath);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        GetFirebaseLink();
        StartCoroutine(DelayStart());
    }

    void Update()
    {
        if(!reset)
        {
            if (Input.GetKeyDown(KeyCode.F5))
                Restart();
        }
    }

    void Restart()
    {
        erroConsole.text = "";
        console.text = "다시 접속합니다.";
        StopAllCoroutines();
        StartCoroutine(DelayStart());
    }

    void InitAnimator()
    {
        ecoPlayerAnimator.enabled = true;
        btnGroupAnimator.enabled = true;
        movieAnimator.enabled = true;
    }

    IEnumerator DelayStart()
    {
        reset = false;
        yield return new WaitForSeconds(1f);
        console.text = "<color='yellow'>링크</color>를 <color='green'>노동요</color>로 변환 중입니다.\n 오래걸릴시 F5를 눌러주세요.";
        pathParser.ExecuteParse();
        yield return SpinWait();
        reset = true;

        console.text = "<color='blue'>ECOPLAYER</color> 를 실행합니다.";
        Destroy(coreContainer.gameObject);
        yield return new WaitForSeconds(2f);

        console.text = "";
        InitAnimator();
        videoPlayer.PlayerStart();
    }

    IEnumerator SpinWait()
    {
        while (ListContainer.Instance.links.Count >= ListContainer.Instance.paths.Count * 2)
            yield return null;
        console.text = "<color='green'>노동요</color>를 리스트에 추가합니다.";
        yield return new WaitForSeconds(3.5f);
    }
}
