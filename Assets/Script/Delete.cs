/* Module name: Delete
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Delete : MonoBehaviour, IPointerClickHandler
{
    public GameObject imageToDelete; // The image that this button will delete
    public Button editButton;
    public GameObject gameObject;
    private TMP_Text editButtonText;
    private int buttonState = 0;
    private Button button;
    private Color color;
    private Image buttonImage;
    void Start()
    {
        Button[] objects = FindObjectsOfType<Button>();
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        color = buttonImage.color;
        foreach (var obj in objects)
        {
            if (obj.name == "CharacterEditButton")
            {
                editButton = obj;
            }
        }
        editButtonText = editButton.GetComponentInChildren<TMP_Text>();
        switch (editButtonText.text)
        {
            case "Unlock":
                buttonState = 0;
                color.a = 0;
                buttonImage.color = color;
                button.interactable = false;
                break;
            case "Resize":
                buttonState = 1;
                color.a = 255;
                buttonImage.color = color;
                button.interactable = true;
                break;
            case "Lock":
                buttonState = 2;  
                color.a = 255;
                buttonImage.color = color;
                button.interactable = true;
                break;
        }
    }
    void Update()
    {
        switch (editButtonText.text)
        {
            case "Unlock":
                buttonState = 0;
                color.a = 0;
                buttonImage.color = color;
                button.interactable = false;
                break;
            case "Resize":
                buttonState = 1;
                color.a = 255;
                buttonImage.color = color;
                button.interactable = true;
                break;
            case "Lock":
                buttonState = 2;
                color.a = 255;
                buttonImage.color = color;
                button.interactable = true;
                break;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (imageToDelete != null&&button.interactable)
        {
            Destroy(imageToDelete);
        }
    }

}
