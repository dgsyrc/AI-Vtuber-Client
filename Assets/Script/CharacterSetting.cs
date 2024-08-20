using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using static LayoutSaveProcessor;

public class CharacterSetting : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    // Start is called before the first frame update
    public RectTransform[] objectTransform;
    public TMP_Dropdown dropdown;
    private Vector3 originalPosition;
    private int buttonState = 0;
    private TMP_Text lockButtonText;
    private Vector3 offset;
    public Sprite[] icons; // 图标数组

    public Button lockButton;

    private Image buttonImage; // 按钮图像组件

    

    void Start()
    {
        lockButtonText = lockButton.GetComponentInChildren<TMP_Text>();
        //objectTransform = GetComponent<Transform>();
        originalPosition = objectTransform[dropdown.value].position;
        lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize":"Lock";
        buttonImage = lockButton.GetComponent<Image>();
        buttonImage.sprite = icons[buttonState];
        string path = Application.persistentDataPath + "/layout.json";
        if (System.IO.File.Exists(path))
        {
            JObject configJson = JObject.Parse(System.IO.File.ReadAllText(path));
            JArray panelLayoutList = (JArray)configJson["Panel"];
            foreach (var panelObject in panelLayoutList)
            {
                if (panelObject["name"].ToString()=="Character")
                {
                    objectTransform[0].localPosition = new Vector3(float.Parse(panelObject["x"].ToString()), float.Parse(panelObject["y"].ToString()),0);
                }
                if (panelObject["name"].ToString() == "Danmaku")
                {
                    objectTransform[1].localPosition = new Vector3(float.Parse(panelObject["x"].ToString()), float.Parse(panelObject["y"].ToString()), 0);
                }
            }


            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogError("Save file not found.");
        }
        // 设置锁定按钮的回调
        lockButton.onClick.AddListener(ToggleLock);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (buttonState==1)
        {
            // 保存初始位置
            originalPosition = objectTransform[dropdown.value].position;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            Vector3 componentWorldPos = objectTransform[dropdown.value].position;
            offset = componentWorldPos - worldMousePos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (buttonState == 1)
        {
            // 在拖动时更新对象的位置
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            newPosition.z = objectTransform[dropdown.value].position.z;  // 保持Z轴不变
            objectTransform[dropdown.value].position = newPosition;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(eventData.position);

            // 根据初始偏移量更新组件位置
            objectTransform[dropdown.value].position = worldMousePos + offset;
        }
    }

    public void ToggleLock()
    {
        buttonState = (buttonState + 1)%3;// 更新按钮的文本显示
        buttonImage.sprite = icons[buttonState];
        lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize" : "Lock";
        if(buttonState == 0)
        {
            LayoutSaveProcessor.SavePanels();
        }
    }
}
