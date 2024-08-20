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
    public RectTransform[] targetTransform; // 需要调整大小的目标 Transform
    //public Transform handle; // 当前拖动的手柄
    public Button lockButton;
    public Sprite[] icons; // 图标数组
    public TMP_Dropdown dropdown;


    private Vector3 initialSize;
    private Vector3 initialMousePosition;
    private float initialAspectRatio;
    private int buttonState = 0;
    private TMP_Text lockButtonText;
    private Image buttonImage; // 按钮图像组件


    void Start()
    {
        // 确保手柄和目标 Transform 已设置
        /*if (handle == null || targetTransform == null)
        {
            Debug.LogError("Handle or Target Transform not assigned.");
        }*/

        // 计算初始宽高比
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
            // 记录拖动开始时的信息
            initialSize = targetTransform[dropdown.value].localScale;
            initialMousePosition = eventData.position;
        }
            
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(buttonState == 2)
        {
            // 计算鼠标当前位置
            Vector3 mousePosition = eventData.position;

            // 计算鼠标移动的距离
            Vector3 delta = (mousePosition - initialMousePosition)/1000f;

            // 计算新的尺寸保持等比例
            float widthDelta = delta.x;
            float heightDelta = delta.y;

            float newWidth = initialSize.x + widthDelta;
            float newHeight = newWidth / initialAspectRatio;

            // 更新目标 Transform 的大小
            targetTransform[dropdown.value].localScale = new Vector3(newWidth, newHeight, targetTransform[dropdown.value].localScale.z);

            // 更新手柄的位置
            //handle.position = mousePosition;
        }
       
    }

    private void ToggleLock()
    {
        buttonState = (buttonState + 1) % 3;// 更新按钮的文本显示
        buttonImage.sprite = icons[buttonState];
        lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize" : "Lock";
    }
}