using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Videos 
{
    public string VideoName;
    public Sprite VideoCover;
    public VideoClip VideoClip;
    public bool IsSelected;

    public VideoClip GetVideoClip()
    {
        return VideoClip;
    }

    public Sprite GetVideoCover()
    {
        return VideoCover;
    }

    public string GetVideoName()
    {
        return VideoName;
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
    }
}
