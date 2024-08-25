using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class TTS : MonoBehaviour
{
    public TMP_InputField inputField;  // ���õ�UI�ı���
    public AudioSource audioSource; // ���õ���ƵԴ
    public string character = "Hutao";        // ��ɫ�趨
    public BilibiliDanmakuFetcher fetcher;

    private string baseUrl = "http://127.0.0.1:5000";
    private bool isStart = false;

    void Start()
    {
        // ��Ӱ�ť����¼�����
        StartCoroutine(audioPlayStatus());
    }

    public void Submit()
    {
        string text = inputField.text;
        if (!string.IsNullOrEmpty(text))
        {
            StartCoroutine(GetTTS(text));
            Debug.Log("�ı��ѷ��ͣ�������ı�Ϊ��" + text);
        }
        else
        {
            Debug.LogWarning("������ı�Ϊ��");
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
