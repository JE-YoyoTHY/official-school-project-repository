using System.Collections.Generic;


[System.Serializable]
public class Subtitle
{
    public string content;
    public float duration;
}

public class SubtitleDict
{
    public Dictionary<string, Subtitle> subtitleSet;
}




