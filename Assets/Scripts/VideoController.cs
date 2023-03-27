using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using System;
using UnityEngine.Events;


public class VideoController : MonoBehaviour
{
    [Header("Video Player UI")]
    public Canvas BgCanvas;
    public CanvasGroup BgCanvasGroups;
    public CanvasGroup BtnsCanvasGroups;

    [Header("Video Player")]
    public VideoPlayer VideoPlayer;
    public bool AutoPlay;

    [Header("Video Player buttons")]
    public Button PlayButton;
    public Button PauseButton;
    public Button StopButton;

    [Header("Events")]
    [SerializeField]
    public CloseEvent CloseEvents;

    [Serializable]
    public class CloseEvent : UnityEvent { }

    public List<Videos> Videos = new List<Videos>();
    private Videos selectedVideo;

    [Header("Video Selection UI")]
    public Transform VideoSelectionUI;
    public GameObject VideoCoverPrefab;

    private void Awake()
    {
        this.VideoPlayer.loopPointReached += ResetPlayer;
        foreach (var video in Videos)
        {
            var videoCoverUI = Instantiate(VideoCoverPrefab, VideoSelectionUI);
            var videoCoverButton = videoCoverUI.GetComponent<Button>();
            var videoCoverImage = videoCoverUI.GetComponent<Image>();
            videoCoverImage.sprite = video.GetVideoCover();
            videoCoverButton.onClick.AddListener(() => OnVideoSelected(video));
        }
    }

    private void ResetPlayer(VideoPlayer player)
    {
        this.VideoPlayer.Stop();

        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = false;
    }

    public void Init(string videoName = null)
    {
        this.HideHUD();

        BgCanvasGroups.interactable = true;
        BgCanvasGroups.blocksRaycasts = true;
        BtnsCanvasGroups.interactable = true;
        BtnsCanvasGroups.blocksRaycasts = true;

        PlayButton.interactable = false;
        PauseButton.interactable = false;
        StopButton.interactable = false;

        if (!string.IsNullOrEmpty(videoName))
        {
            var video = Videos.Find(v => v.GetVideoName() == videoName);
            if (video != null)
            {
                OnVideoSelected(video);
                OnPlayPressed();
            }
        }
    }

    private void OnVideoSelected(Videos video)
    {
        if (selectedVideo != null)
        {
            selectedVideo.SetSelected(false);
        }

        selectedVideo = video;
        selectedVideo.SetSelected(true);

        VideoPlayer.clip = selectedVideo.GetVideoClip();

        if (AutoPlay)
        {
            OnPlayPressed();
        }
        else
        {
            PlayButton.interactable = true;
            PauseButton.interactable = false;
            StopButton.interactable = false;
        }
    }

    public void OnPlayPressed()
    {
        if (this.VideoPlayer.clip == null && string.IsNullOrEmpty(this.VideoPlayer.url)) return;
        if (this.VideoPlayer.isPlaying) return;
        if (BgCanvasGroups.alpha != 1f) return;

        this.VideoPlayer.Play();

        PlayButton.interactable = false;
        PauseButton.interactable = true;
        StopButton.interactable = true;
    }

    public void OnPausePressed()
    {
        if (!this.VideoPlayer.isPlaying) return;
        if (BgCanvasGroups.alpha != 1f) return;

        this.VideoPlayer.Pause();

        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = true;
    }

    public void OnStopPressed()
    {
        if (!this.VideoPlayer.isPlaying && !this.VideoPlayer.isPaused) return;
        if (BgCanvasGroups.alpha != 1f) return;

        this.VideoPlayer.Stop();

        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = false;
    }

    public void OnBackPressed()
    {
        if (BgCanvasGroups.alpha != 1f) return;

        this.VideoPlayer.Stop();

        PlayButton.interactable = false;
        PauseButton.interactable = false;
        StopButton.interactable = false;

        BtnsCanvasGroups.interactable = false;
        BtnsCanvasGroups.blocksRaycasts = false;

        if (CloseEvents.GetPersistentEventCount() != 0) CloseEvents.Invoke();

        this.BgCanvasGroups.DOFade(0f, 0.5f);
        this.BtnsCanvasGroups.DOFade(0f, 0.5f).OnComplete(() =>
        {
            BgCanvasGroups.interactable = false;
            BgCanvasGroups.blocksRaycasts = false;
            this.ShowHUD();
        });
    }

    private void HideHUD()
    {
        BgCanvas.enabled = false;
        BtnsCanvasGroups.alpha = 0f;
        BtnsCanvasGroups.interactable = false;
        BtnsCanvasGroups.blocksRaycasts = false;
    }

    private void ShowHUD()
    {
        BgCanvas.enabled = true;
        BtnsCanvasGroups.alpha = 1f;
        BtnsCanvasGroups.interactable = true;
        BtnsCanvasGroups.blocksRaycasts = true;
    }

}




