using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DanmakuScroller : MonoBehaviour
{
    //public TextMeshProUGUI danmakuPrefab;  // ������ʾ��Ļ��Ԥ����
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
    public RectTransform contentParent;        // ���ڷ��õ�Ļ�ĸ�����
    public ScrollRect scrollRect;          // ������ͼ
    public float displayInterval = 1f;     // ��Ļ��ʾ���ʱ��
    public float padding = 0.0f; // ��Ļ֮��ļ��

    public Sprite[] badge; // ��˿������
    public Sprite[] guardIcon; // �󺽺�ͼ��

    private Queue<danmakuData> danmakuQueue = new Queue<danmakuData>();  // �洢��Ļ���ݵĶ���

    void Start()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        // ��ʼ��ʱ�������ݸ߶�
        UpdateContentHeight();
        StartCoroutine(DisplayDanmaku());
    }

    // ��ӵ�Ļ��������
    public void AddDanmaku(danmakuData danmakuAddData)
    {
        danmakuQueue.Enqueue(danmakuAddData);
    }
    private void UpdateContentHeight()
    {
        // �������ݸ߶�
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

        // �������ݵĸ߶�
        contentParent.sizeDelta = new Vector2(0f, totalHeight);
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private void SendPrompt()
    {

    }

    // ��ʾ��Ļ��Э��
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
                // �Զ���������ײ�
                /*Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();*/
                // �ȴ� 1 ������ʾ��һ����Ļ
                yield return new WaitForSeconds(displayInterval);
                UpdateContentHeight();
            }
            else
            {
                // ���û�е�Ļ�ȴ� 0.1 ��
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
