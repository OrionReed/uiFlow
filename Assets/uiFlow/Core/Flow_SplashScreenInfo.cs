using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.UI;

[Serializable]
public struct Flow_SplashScreenInfo
{
    public enum SplashType
    {
        Image,
        Video
    }

    public SplashType Type;
    public Texture2D Image;
    public VideoClip Video;
    public float In;
    public float Sustain;
    public float Out;
}