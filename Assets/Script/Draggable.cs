using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

public class Draggable : MonoBehaviour, IDragHandler
{
    public Button editButton;
    public Sprite[] icons; // 图标数组

    private RectTransform rectTransform;
    private Canvas canvas;
    private int buttonState = 0;
    private Image buttonImage; // 按钮图像组件
    public TMP_Text lockButtonText;
    private Vector3 initialMousePosition;
    private Vector3 initialSize;
    private float initialAspectRatio;


    //private CanvasGroup canvasGroup;

    void Start()
    {
        Button[] objects = FindObjectsOfType<Button>();
        foreach(var obj in objects)
        {
            if(obj.name == "CharacterEditButton")
            {
                editButton = obj;
            }
        }
        lockButtonText = editButton.GetComponentInChildren<TMP_Text>();
        switch(lockButtonText.text)
        {
            case "Unlock":
                buttonState = 0;
                break;
            case "Resize":
                buttonState = 1;
                break;
            case "Lock":
                buttonState = 2;
                break;
        }
        //lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize" : "Lock";
        buttonImage = editButton.GetComponent<Image>();
        //buttonImage.sprite = icons[buttonState];
        initialAspectRatio = rectTransform.localScale.x / rectTransform.localScale.y;
        editButton.onClick.AddListener(ToggleEdit);
    }
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        //canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        switch (buttonState)
        {
            case 0:
                break;
            case 1:
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
                break;
            case 2:
                // 计算鼠标当前位置
                Vector3 mousePosition = eventData.position;

                // 计算鼠标移动的距离
                Vector3 delta = (mousePosition - initialMousePosition) / 1000f;

                // 计算新的尺寸保持等比例
                float widthDelta = delta.x;
                float heightDelta = delta.y;

                float newWidth = initialSize.x + widthDelta;
                float newHeight = newWidth / initialAspectRatio;

                // 更新目标 Transform 的大小
                rectTransform.localScale = new Vector3(newWidth, newHeight, rectTransform.localScale.z);
                break;
            default:
                break;
        }
        
    }

    private void ToggleEdit()
    {
        buttonState = (buttonState + 1) % 3;// 更新按钮的文本显示
        buttonImage.sprite = icons[buttonState];
        lockButtonText.text = (buttonState == 0) ? "Unlock" : (buttonState == 1) ? "Resize" : "Lock";
    }

    /*public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

  

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }*/
}
