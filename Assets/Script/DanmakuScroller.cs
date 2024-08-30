/* Module name: DanmakuScroller 
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DanmakuScroller : MonoBehaviour
{
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
    public int count = 11;

    public Sprite[] badge; // ��˿������
    public Sprite[] guardIcon; // �󺽺�ͼ��

    private Queue<danmakuData> danmakuQueue = new Queue<danmakuData>();  // �洢��Ļ���ݵĶ���
    private float totalHeight = 0f;
    private Queue<string> damakuList = new Queue<string>();

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
        int childCount = contentParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            RectTransform rt = contentParent.GetChild(i).GetComponent<RectTransform>();
            if (rt != null)
            {
                totalHeight += 35f + padding;
            }
        }

        // �������ݵĸ߶�
        contentParent.sizeDelta = new Vector2(0f, totalHeight);
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
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
                if (damakuList.Count < count)
                {
                    totalHeight += 40f;
                    contentParent.sizeDelta = new Vector2(0f, totalHeight);
                    scrollRect.verticalNormalizedPosition = 0f;
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    string destoryName = damakuList.Dequeue();
                    GameObject objToDestroy = GameObject.Find(destoryName);
                    if (objToDestroy != null)
                    {
                        // ���ٶ���
                        Destroy(objToDestroy);
                    }
                    else
                    {
                        Debug.LogWarning("δ�ҵ�ָ�����ƵĶ���");
                    }
                }
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
                damakuList.Enqueue(newDanmaku.name);


                UnityEngine.Debug.LogError("text:"+textComponent.text);

                // �ȴ�һ��ʱ����ʾ��һ����Ļ
                yield return new WaitForSeconds(displayInterval);
                //UpdateContentHeight();
            }
            else
            {
                // ���û�е�Ļ�ȴ� 0.1 ��
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
