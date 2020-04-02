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
    public bool overlap;

    public Link(string youtubeLink, string user, bool overlap)
    {
        this.youtubeLink = youtubeLink;
        this.user = user;
        this.overlap = overlap;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["youtubeLink"] = youtubeLink;
        result["user"] = user;
        result["overlap"] = overlap;
        return result;
    }
}

public class FirebaseExecuter : MonoBehaviour
{
    [SerializeField] Text console;
    [SerializeField] PathParser pathParser;
    [SerializeField] Animator ecoPlayerAnimator;
    [SerializeField] Animator btnGroupAnimator;
    [SerializeField] Animator movieAnimator;

    public const string firebasePath = "https://ecoplayer-64d9d.firebaseio.com/";
    public DatabaseReference reference;
    string FindLink(string allMessage)
    {
        char[] sep = { '\n', '\t', ' ' };
        string[] result = allMessage.Split(sep);
        foreach (var item in result)
        {
            if (item.Contains("https://www.youtube.com/watch"))
            {
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
            {
                Debug.Log("failed");
            }
            else if (task.IsCompleted)
            {
                Firebase.Database.DataSnapshot snapshot = task.Result;
                foreach (var childSnapshot in snapshot.Children)
                {
                    if (!bool.Parse(childSnapshot.Child("overlap").Value.ToString()))
                    {
                        var link = childSnapshot.Child("youtubeLink").Value.ToString();
                        link = FindLink(link);
                        if (link == null)
                            reference.Child("ecoplayer").Child(childSnapshot.Key.ToString()).RemoveValueAsync();
                        else
                            ListContainer.Instance.links.Add(new KeyValuePair<string, string>(childSnapshot.Key.ToString(), link));
                    }
                }
            }
        });
    }

    void GetFirebasePath()
    {
        FirebaseDatabase.DefaultInstance.GetReference("ecoplayer").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("failed");
            }
            else if (task.IsCompleted)
            {
                Firebase.Database.DataSnapshot snapshot = task.Result;
                foreach (var childSnapshot in snapshot.Children)
                {
                    if (bool.Parse(childSnapshot.Child("overlap").Value.ToString()))
                    {
                        var path = childSnapshot.Child("youtubeLink").Value.ToString();
                        ListContainer.Instance.playerPath.Add(path);
                    }
                }
            }
        });
    }

    public void SetFirebasePath()
    {
        for (int i = 0; i < ListContainer.Instance.names.Count; i++)
        {
            reference.Child("ecoplayer").Child(ListContainer.Instance.names[i]).Child("overlap").SetValueAsync(true);
            reference.Child("ecoplayer").Child(ListContainer.Instance.names[i]).Child("youtubeLink").SetValueAsync(ListContainer.Instance.paths[i]);
        }
        ListContainer.Instance.names.Clear();
        ListContainer.Instance.paths.Clear();
    }

    void Awake()
    {
        Screen.SetResolution(600, 450, false);
    }

    void Start()
    {
        console.text = "목록을 받아오고있습니다.";
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(firebasePath);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        GetFirebaseLink();
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(1f);
        if (ListContainer.Instance.links.Count > 0)
        {
            console.text = "추가된 곡을 찾았습니다";
            pathParser.ExecuteParese();
            yield return SpinLock();
        }
        GetFirebasePath();
        console.text = "<color='blue'>ECOPLAYER</color> 를 실행합니다";
        yield return new WaitForSeconds(2f);
        console.text = "";
        ecoPlayerAnimator.enabled = true;
        btnGroupAnimator.enabled = true;
        movieAnimator.enabled = true;
    }

    IEnumerator SpinLock()
    {
        while (ListContainer.Instance.paths.Count == 0 ||
            ListContainer.Instance.names.Count != ListContainer.Instance.paths.Count)
        {
            console.text = "추가된 곡을 변환중입니다.";
            yield return null;
        }
        SetFirebasePath();
        console.text = "리스트에 추가합니다.";
        yield return new WaitForSeconds(2f);
    }
}
