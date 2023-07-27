using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncSlideWithSlideAndroid : MonoBehaviour
{
    //[SerializeField] private Slider sliderVolume;
    //[SerializeField] private float sliderVal;
    //[SerializeField] public GameObject panelSlider;
    //[SerializeField] private Text volumeText;
    private float _lastVolume = -1f;
    //[SerializeField] public GameObject panelSliderCall;
    [SerializeField] private Slider sliderVolumeCall;
    [SerializeField] private Text volumeTextCall;
    [SerializeField] private float sliderValCall;
    //[SerializeField] public GameObject panelSliderCallLiver;
    [SerializeField] private Slider sliderVolumeCallLiver;
    [SerializeField] private Text volumeTextCallLiver;
    [SerializeField] private float sliderValCallLiver;

    private void Start()
    {
        //sliderVolume.onValueChanged.AddListener(OnVolumeSliderChanged);
        sliderVolumeCallLiver.onValueChanged.AddListener(OnCallSliderChanged);
#if UNITY_EDITOR 
        //LoadSliderValue();
        LoadSliderValueCallLiver();
#endif
        //volumeText.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolume.value * 100f).ToString() + "%";
        volumeTextCallLiver.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolumeCallLiver.value * 100f).ToString() + "%";
        sliderVolumeCall.onValueChanged.AddListener(OnVolumeSliderCallChanged);
        volumeTextCall.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolumeCall.value * 100f).ToString() + "%";
        LoadSliderCallValue();
    }

    private void Update()
    {
        volumeTextCall.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolumeCall.value * 100f).ToString() + "%";
// #if UNITY_ANDROID && !UNITY_EDITOR
//         var currentVolume =  AndroidNativeVolumeService.GetSystemVolume();
//         if (currentVolume == lastVolume) return;
//         sliderVolume.value = currentVolume;
//         lastVolume = currentVolume;
// #endif
//         volumeText.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolume.value * 100f).ToString() + "%";
#if UNITY_ANDROID && !UNITY_EDITOR
        var currentVolume =  AndroidNativeVolumeService.GetSystemVolumeCall();
        if (currentVolume == lastVolume) return;
        sliderVolumeCallLiver.value = currentVolume;
        lastVolume = currentVolume;
        if (currentVolume == 0)
        {
            volumeTextCallLiver.text = "Volume Percent: " +  "10%";
        }
        else
             volumeTextCallLiver.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolumeCallLiver.value * 100f).ToString() + "%";
#endif
#if UNITY_EDITOR 
        volumeTextCallLiver.text = "Volume Percent: " + Mathf.RoundToInt(sliderVolumeCallLiver.value * 100f).ToString() + "%";
#endif
    }
    private void OnVolumeSliderChanged(float value)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNativeVolumeService.SetSystemVolume(value);
#endif
#if UNITY_EDITOR
        //AudioListener.volume = sliderVolume.value;
#endif
    }
    private void OnCallSliderChanged(float value)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNativeVolumeService.SetSystemVolumeCall(value);
#endif
#if UNITY_EDITOR
        AudioListener.volume = sliderVolumeCallLiver.value;
#endif
    }
// #if UNITY_EDITOR
//     public void SaveSliderValue(float value)
//     {
//         sliderVal = value; 
//         PlayerPrefs.SetFloat("SliderValue", value);
//         PlayerPrefs.Save();
//     }
//
//     private void LoadSliderValue() 
//     {
//         var savedValue = PlayerPrefs.GetFloat("SliderValue", sliderVal);
//         sliderVolume.value = savedValue;
//     }
// #endif
#if UNITY_2021_3_6
    public void SaveSliderValueCallLiver(float value)
    {
        sliderValCallLiver = value; 
        PlayerPrefs.SetFloat("SliderValueCallLiver", value);
        PlayerPrefs.Save();
    }

    private void LoadSliderValueCallLiver() 
    {
        var savedValue = PlayerPrefs.GetFloat("SliderValueCallLiver", sliderValCallLiver);
        sliderVolumeCallLiver.value = savedValue;
    }
#endif
    public void SaveSliderCallValue(float value)
    {
        sliderValCall = value; 
        PlayerPrefs.SetFloat("SliderValueCall", value);
        PlayerPrefs.Save();
    }

    private void LoadSliderCallValue() 
    {
        var savedValue = PlayerPrefs.GetFloat("SliderValueCall", sliderValCall);
        sliderVolumeCall.value = savedValue;
    }

    private void OnVolumeSliderCallChanged(float value)
    {
        AudioListener.volume = sliderVolumeCall.value;
    }
}

#if UNITY_ANDROID && !UNITY_EDITOR
public class AndroidNativeVolumeService
    {
        static int STREAMMUSIC;
        static int STREAMMUSICCALL;
        static int FLAGSHOWUI = 1;
 
        public static AndroidJavaObject audioManager;
 
        public static AndroidJavaObject deviceAudio
        {
            get
            {
                if (audioManager != null) return audioManager;
                var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                var audioManagerClass = new AndroidJavaClass("android.media.AudioManager");
                var contextClass = new AndroidJavaClass("android.content.Context");
 
                STREAMMUSIC = audioManagerClass.GetStatic<int>("STREAM_MUSIC");
                STREAMMUSICCALL = audioManagerClass.GetStatic<int>("STREAM_VOICE_CALL");
                var Context_AUDIO_SERVICE = contextClass.GetStatic<string>("AUDIO_SERVICE");
 
                audioManager = context.Call<AndroidJavaObject>("getSystemService", Context_AUDIO_SERVICE);

                Debug.Log(audioManager != null
                    ? "[AndroidNativeVolumeService] Android Audio Manager successfully set up"
                    : "[AndroidNativeVolumeService] Could not read Audio Manager");
                return audioManager;
            }
 
        }
 
        private static int GetDeviceMaxVolume()
        {
            return deviceAudio.Call<int>("getStreamMaxVolume", STREAMMUSIC);
        }
        private static int GetDeviceMaxVolumeCall()
        {
            return deviceAudio.Call<int>("getStreamMaxVolume", STREAMMUSICCALL);
        }
 
        public static float GetSystemVolume()
        {
            var deviceVolume = deviceAudio.Call<int>("getStreamVolume", STREAMMUSIC);
            var scaledVolume = (float)(deviceVolume / (float)GetDeviceMaxVolume());
 
            return scaledVolume;
        }
 
        public static void SetSystemVolume(float volumeValue)
        {
            var scaledVolume = (int)(volumeValue * (float)GetDeviceMaxVolume());
            deviceAudio.Call("setStreamVolume", STREAMMUSIC, scaledVolume, FLAGSHOWUI);
        }
        public static float GetSystemVolumeCall()
        {
            var deviceVolume = deviceAudio.Call<int>("getStreamVolume", STREAMMUSICCALL);
            var scaledVolume = (float)(deviceVolume / (float)GetDeviceMaxVolumeCall());
 
            return scaledVolume;
        }
 
        public static void SetSystemVolumeCall(float volumeValue)
        {
            var scaledVolume = (int)(volumeValue * (float)GetDeviceMaxVolumeCall());
            deviceAudio.Call("setStreamVolume", STREAMMUSICCALL, scaledVolume, 0);
        }
    }
#endif
