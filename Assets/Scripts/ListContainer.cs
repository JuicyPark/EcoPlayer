using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListContainer : Singleton<ListContainer>
{
    public List<KeyValuePair<string,string>> links = new List<KeyValuePair<string, string>>();
    public List<string> names = new List<string>();
    public List<string> paths = new List<string>();
    public List<string> playerPath = new List<string>();
}
