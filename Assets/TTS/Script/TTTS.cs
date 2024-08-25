using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class TTS : MonoBehaviour
{
    public TMP_InputField inputField;  // 引用到UI文本框
    public AudioSource audioSource; // 引用到音频源
    public string character = "Hutao";        // 角色设定
    public BilibiliDanmakuFetcher fetcher;

    private string baseUrl = "http://127.0.0.1:5000";
    private bool isStart = false;

    void Start()
    {
        // 添加按钮点击事件监听
        StartCoroutine(audioPlayStatus());
    }

    public void Submit()
    {
        string text = inputField.text;
        if (!string.IsNullOrEmpty(text))
        {
            StartCoroutine(GetTTS(text));
            Debug.Log("文本已发送，输入的文本为：" + text);
        }
        else
        {
            Debug.LogWarning("输入的文本为空");
        }
    }
    IEnumerator GetTTS(string text)
    {
        string url = $"{baseUrl}/tts?character={UnityWebRequest.EscapeURL(character)}&text={UnityWebRequest.EscapeURL(text)}";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                yield break;
            }

            byte[] audioData = www.downloadHandler.data;
            AudioClip audioClip = WavUtility.ToAudioClip(audioData);
            audioSource.clip = audioClip;
            audioSource.Play(); 
            isStart = true;
        }
    }
    IEnumerator audioPlayStatus()
    {
        while (true)
        {
            if (isStart)
            {
                yield return new WaitForSeconds(0.5f);
                if (!audioSource.isPlaying)
                {
                    isStart = false;
                    fetcher.SetIDLE();
                }
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
    }
}
