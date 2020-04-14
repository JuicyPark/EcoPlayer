using System.Collections.Generic;

public class ListContainer : Singleton<ListContainer>
{
    public List<string> links = new List<string>();
    public List<PathContainer> paths = new List<PathContainer>();
}
