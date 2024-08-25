using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DanmakuScroller : MonoBehaviour
{
    //public TextMeshProUGUI danmakuPrefab;  // 用于显示弹幕的预制体
    public struct danmakuData
    {
       public string name;
       public string text;
       public int badge;
       public string badge_name;
       public int guard_level;
    };
    public GameObject danmakuPrefab;
    public GameObject danmakuNoBadgePrefab;
    public RectTransform contentParent;        // 用于放置弹幕的父物体
    public ScrollRect scrollRect;          // 滚动视图
    public float displayInterval = 1f;     // 弹幕显示间隔时间
    public float padding = 0.0f; // 弹幕之间的间隔

    public Sprite[] badge; // 粉丝牌数组
    public Sprite[] guardIcon; // 大航海图标

    private Queue<danmakuData> danmakuQueue = new Queue<danmakuData>();  // 存储弹幕内容的队列

    void Start()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        // 初始化时设置内容高度
        UpdateContentHeight();
        StartCoroutine(DisplayDanmaku());
    }

    // 添加弹幕到队列中
    public void AddDanmaku(danmakuData danmakuAddData)
    {
        danmakuQueue.Enqueue(danmakuAddData);
    }
    private void UpdateContentHeight()
    {
        // 计算内容高度
        float totalHeight = 0f;
        int childCount = contentParent.childCount;
        //Debug.LogError(childCount.ToString());
        for (int i = 0; i < childCount; i++)
        {
            RectTransform rt = contentParent.GetChild(i).GetComponent<RectTransform>();
            if (rt != null)
            {
                totalHeight += 30f + padding;
            }
        }

        // 设置内容的高度
        contentParent.sizeDelta = new Vector2(0f, totalHeight);
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private void SendPrompt()
    {

    }

    // 显示弹幕的协程
    IEnumerator DisplayDanmaku()
    {
        while (true)
        {
            if (danmakuQueue.Count > 0)
            {
                danmakuData newData = danmakuQueue.Dequeue();
                GameObject newDanmaku;
                TMP_Text textComponent;
                TMP_Text badgeNameText;
                Image imageBadge;
                Image imageGuard;
                if (newData.badge == 0) 
                {
                    newDanmaku = Instantiate(danmakuNoBadgePrefab, contentParent);
                    textComponent = newDanmaku.GetComponentInChildren<TMP_Text>();
                    UnityEngine.Debug.LogError("Init");
                }
                else 
                {
                    newDanmaku = Instantiate(danmakuPrefab, contentParent);
                    textComponent = newDanmaku.GetComponentInChildren<TMP_Text>();
                    badgeNameText = newDanmaku.GetComponentInChildren<Image>().GetComponentInChildren<TMP_Text>();
                    //imageBadge = newDanmaku.GetComponentInChildren<Image>();
                    foreach(Image img in newDanmaku.GetComponentsInChildren<Image>())
                    {
                        if(img.name == "Image")
                        {
                            img.sprite = badge[newData.badge];
                        }
                        if(img.name =="guardIcon")
                        {
                            img.sprite = guardIcon[newData.guard_level];
                        }
                    }
                    //imageBadge.sprite = badge[newData.badge];
                    badgeNameText.text = newData.badge_name;
                }
                //string danmakuText = danmakuQueue.Dequeue();
                
                
                textComponent.text = newData.name+": "+newData.text;


                UnityEngine.Debug.LogError("text:"+textComponent.text);
                // 自动滚动到最底部
                /*Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();*/
                // 等待 1 秒再显示下一条弹幕
                yield return new WaitForSeconds(displayInterval);
                UpdateContentHeight();
            }
            else
            {
                // 如果没有弹幕等待 0.1 秒
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
