using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using TMPro;
using System.Text.RegularExpressions;
using static UnityClient;


public class BilibiliDanmakuFetcher : MonoBehaviour
{
    
    public string roomID = "0"; // 替换为你的直播间ID
    public ScrollRect scrollRect; // 滚动视图
    public TMP_InputField roomIDInput;
    private string danmakuApiUrl = "https://api.live.bilibili.com/xlive/web-room/v1/dM/gethistory?roomid=";
    private string heartBeatApiUrl = "https://api.live.bilibili.com/relation/v1/Feed/heartBeat";
    public DanmakuScroller danmakuScroller;
    public float UpdateIntervalTime = 5f;
    public TMP_Text Ask;
    public TMP_InputField Ans;
    public TMP_InputField AnsWithEmoji;
    private UnityClient.AIInterface aiInterface;

    private int lastTs = 0;
    private string lastAnswer=" ";
    private string nowAnswer;
    private string lastAsk=" ";
    private bool isLLMIdle = true;

    void Start()
    {
        UnityClient client = FindObjectOfType<UnityClient>();  // 获取UnityClient实例
        aiInterface = new UnityClient.AIInterface(client);  // 创建AIInterface实例
        StartCoroutine(FetchHeartBeat());
        StartCoroutine(FetchDanmaku());
    }

    IEnumerator FetchHeartBeat()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(heartBeatApiUrl);
            //SetHeartBeatHeaders(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error in heartBeat: " + request.error);
            }

            yield return new WaitForSeconds(2f); // 每2秒发送一次心跳
        }
    }

    IEnumerator FetchDanmaku()
    {
        while (true)
        {
            Debug.LogError(roomID);
            if (int.Parse(roomID) == 0)
            {
                yield return new WaitForSeconds(UpdateIntervalTime); // 每2秒获取一次弹幕
                continue;
            }
            string url = danmakuApiUrl + roomID + "&room_type=0";
            UnityWebRequest request = UnityWebRequest.Get(url);
            //SetDanmakuHeaders(request);
            Debug.LogError("wait ans");
            yield return request.SendWebRequest();
            Debug.LogError("get ans");
            if (request.result == UnityWebRequest.Result.Success)
            {
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                SaveJsonToFile(request.downloadHandler.text);
                ParseAndDisplayDanmaku(jsonResponse);
            }
            else
            {
                Debug.LogError("Error fetching danmaku: " + request.error);
            }
            nowAnswer = aiInterface.GetAnswer();
            Debug.LogError("Ans:" + nowAnswer);
            Debug.LogError("ifAns:"+(lastAnswer != nowAnswer).ToString());
            Debug.LogError("askinfo:"+aiInterface.GetChosenText());
            if (lastAnswer!=nowAnswer&&nowAnswer!=null)
            {
                Ask.text = aiInterface.GetChosenText();
                Ans.text = RemoveBrackets(nowAnswer);
                AnsWithEmoji.text = nowAnswer;
                lastAnswer = nowAnswer;
                lastAsk = aiInterface.GetChosenText();
                isLLMIdle = true;
                aiInterface.SetIdle();
                //aiInterface.SetPrompt("null");
            }
            /*else
            {
                if(lastAsk == aiInterface.GetChosenText())
                {
                    isLLMIdle=true;
                }
            }*/

            yield return new WaitForSeconds(UpdateIntervalTime); // 每2秒获取一次弹幕
        }
    }


    void ParseAndDisplayDanmaku(JObject jsonResponse)
    {
        JArray danmakuList = (JArray)jsonResponse["data"]["room"];

        //Debug.LogError("Error 1");
        foreach (var danmaku in danmakuList)
        {
            string text = danmaku["text"].ToString();
            string nickname = danmaku["nickname"].ToString();
            string timeline = danmaku["timeline"].ToString();
            string tsStr = danmaku["check_info"]["ts"].ToString();
            string guardLevelStr = danmaku["guard_level"].ToString(); 
            string badgeLevel;
            string badgeText;
            JArray medalArray = (JArray)danmaku["medal"];
            
            if (medalArray.Count == 0)
            {
                badgeLevel = "0";
                badgeText = null;
            }
            else
            {
                badgeLevel = danmaku["medal"][0].ToString();
                badgeText = danmaku["medal"][1].ToString();

            }
            
            //Debug.LogError(tsStr);
            int ts = int.Parse(tsStr);
            int badgeLevelInt = int.Parse(badgeLevel);
            int guardLevel = int.Parse(guardLevelStr);
            
            if (ts>lastTs)
            {
                lastTs = ts;
                DanmakuScroller.danmakuData tmpData = new DanmakuScroller.danmakuData();
                tmpData.name = nickname;
                tmpData.text = $"{text}\n";
                tmpData.badge = badgeLevelInt;
                tmpData.badge_name = badgeText;
                tmpData.guard_level = guardLevel;
                danmakuScroller.AddDanmaku(tmpData);
                //LLM部分
                Debug.LogError("isllmIDLE:" + isLLMIdle.ToString()+" Text: "+text);
                if (isLLMIdle)
                {
                    Debug.LogError("Ask:" + text);
                    aiInterface.SetPrompt(text);
                    Debug.LogError("setaskinfo:" + aiInterface.GetChosenText());
                    aiInterface.SetTsID(ts);
                    Debug.LogError(text);
                    //lastAnswer = nowAnswer;
                    isLLMIdle = false;
                    
                }
            }
        }
        
    }

    void SaveJsonToFile(string jsonContent)
    {
        string persistentPath = UnityEngine.Application.persistentDataPath;
        string fileName = "danmakuData.json";
        string filePath = Path.Combine(persistentPath, fileName);

        File.WriteAllText(filePath, jsonContent);
    }

    public void SaveID()
    {
        lastTs = 0;
        roomID = roomIDInput.text;
    }

    public static string RemoveBrackets(string input)
    {
        return Regex.Replace(input, @"\s*(\(.*?\))\s*", "", RegexOptions.Singleline);
    }
}
