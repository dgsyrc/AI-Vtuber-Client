using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    public Button saveButton; // Assign this in the Inspector
    public ImageManager imageManager; // Assign this in the Inspector

    void Start()
    {
        saveButton.onClick.AddListener(OnSaveButtonClick);
    }

    void OnSaveButtonClick()
    {
        imageManager.SaveImages(); // Call the instance method
    }
}
