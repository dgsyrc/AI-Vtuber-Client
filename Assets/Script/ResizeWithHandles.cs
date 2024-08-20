using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Newtonsoft.Json.Linq;
public class ResizeHandle : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform[] targetTransform; // ��Ҫ������С��Ŀ�� Transform
    //public Transform handle; // ��ǰ�϶����ֱ�
    public Button lockButton;
    public Sprite[] icons; // ͼ������
    public TMP_Dropdown dropdown;


    private Vector3 initialSize;
    private Vector3 initialMousePosition;
    private float initialAspectRatio;
    private int buttonState = 0;
    private TMP_Text lockButtonText;
    private Image buttonImage; // ��ťͼ�����


    void Start()
    {
        // ȷ���ֱ���Ŀ�� Transform ������
        /*if (handle == null || targetTransform == null)
        {
            Debug.LogError("Handle or Target Transform not assigned.");
        }*/

        // �����ʼ��߱�
        lockButtonText = lockButton.GetComponentInChildren<TMP_Text>();
        initialAspectRatio = targetTransform[dropdown.value].localScale.x / targetTransform[dropdown.value].localScale.y;
        lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize" : "Lock";
        buttonImage = lockButton.GetComponent<Image>();
        buttonImage.sprite = icons[buttonState];
        string path = Application.persistentDataPath + "/layout.json";
        if (System.IO.File.Exists(path))
        {
            JObject configJson = JObject.Parse(System.IO.File.ReadAllText(path));
            JArray panelLayoutList = (JArray)configJson["Panel"];
            foreach (var panelObject in panelLayoutList)
            {
                if (panelObject["name"].ToString() == "Character")
                {
                    targetTransform[0].localScale = new Vector3(float.Parse(panelObject["scaleX"].ToString()), float.Parse(panelObject["scaleY"].ToString()), float.Parse(panelObject["scaleZ"].ToString()));
                }
                if (panelObject["name"].ToString() == "Danmaku")
                {
                    targetTransform[1].localScale = new Vector3(float.Parse(panelObject["scaleX"].ToString()), float.Parse(panelObject["scaleY"].ToString()), float.Parse(panelObject["scaleZ"].ToString()));
                }
            }


            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogError("Save file not found.");
        }
        lockButton.onClick.AddListener(ToggleLock);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (buttonState == 2)
        {
            // ��¼�϶���ʼʱ����Ϣ
            initialSize = targetTransform[dropdown.value].localScale;
            initialMousePosition = eventData.position;
        }
            
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(buttonState == 2)
        {
            // ������굱ǰλ��
            Vector3 mousePosition = eventData.position;

            // ��������ƶ��ľ���
            Vector3 delta = (mousePosition - initialMousePosition)/1000f;

            // �����µĳߴ籣�ֵȱ���
            float widthDelta = delta.x;
            float heightDelta = delta.y;

            float newWidth = initialSize.x + widthDelta;
            float newHeight = newWidth / initialAspectRatio;

            // ����Ŀ�� Transform �Ĵ�С
            targetTransform[dropdown.value].localScale = new Vector3(newWidth, newHeight, targetTransform[dropdown.value].localScale.z);

            // �����ֱ���λ��
            //handle.position = mousePosition;
        }
       
    }

    private void ToggleLock()
    {
        buttonState = (buttonState + 1) % 3;// ���°�ť���ı���ʾ
        buttonImage.sprite = icons[buttonState];
        lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize" : "Lock";
    }
}