using System.Collections.Generic;
using UnityEngine;
using TMPro; // 引入 TextMeshPro 的命名空间
using UnityEngine.UI;
using System.IO;
using SFB; // 引入 Standalone File Browser 的命名空间

public class BackgroundImageManager : MonoBehaviour
{
    public Button selectFileButton; // 选择文件按钮（TMP_Button）
    public TMP_Text filePathText; // 显示文件路径的文本（TMP_Text）
    public TMP_Dropdown modeDropdown; // 显示模式下拉菜单（TMP_Dropdown）
    public Button applyButton; // 应用按钮（TMP_Button）
    public Image backgroundImage; // 背景图组件
    public string savePath; // 保存路径
    public BackgroundModeSetter backgroundModeSetter; // 背景模式设置器

    private Sprite loadedSprite;

    void Start()
    {
        // 设置下拉菜单的选项
        savePath = Path.Combine(Application.persistentDataPath, "BackgroundSettings.json");
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(new List<string> { "平铺", "填充" });

        // 绑定按钮事件
        selectFileButton.onClick.AddListener(OpenFileDialog);
        applyButton.onClick.AddListener(ApplyBackgroundImage);

        // 加载保存的设置
        LoadSettings();
    }

    void OpenFileDialog()
    {
        // 打开文件选择窗口
        var extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )};
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);

        if (paths.Length > 0)
        {
            string path = paths[0];
            filePathText.text = path;
            LoadImage(path);
        }
    }

    void LoadImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            loadedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            backgroundModeSetter.SetBackgroundImage(loadedSprite);
            //backgroundImage.sprite = loadedSprite;
            //backgroundImage.color = Color.white;
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }

    void ApplyBackgroundImage()
    {
        // 应用显示模式
        //ApplyDisplayMode((BackgroundImageMode)modeDropdown.value);
        BackgroundModeSetter.BackgroundImageMode mode = (BackgroundModeSetter.BackgroundImageMode)modeDropdown.value;
        backgroundModeSetter.ApplyDisplayMode(mode);

        // 保存设置
        SaveSettings();
    }

    /*void ApplyDisplayMode(BackgroundImageMode mode)
    {
        switch (mode)
        {
            case BackgroundImageMode.Fill:
                // 设置 Image 组件为填充模式
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = true;
                break;

            case BackgroundImageMode.Tile:
                // 设置 Image 组件为平铺模式
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = false;
                backgroundImage.material = new Material(Shader.Find("Unlit/Texture"));
                backgroundImage.material.mainTexture = loadedSprite.texture;
                backgroundImage.sprite = null; // 确保不使用 Sprite
                break;
        }
    }*/

    void SaveSettings()
    {
        BackgroundSettings settings = new BackgroundSettings
        {
            imagePath = filePathText.text,
            mode = (BackgroundImageMode)modeDropdown.value
        };
        string json = JsonUtility.ToJson(settings);
        File.WriteAllText(savePath, json);
    }

    void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            BackgroundSettings settings = JsonUtility.FromJson<BackgroundSettings>(json);
            filePathText.text = settings.imagePath;
            modeDropdown.value = (int)settings.mode;
            LoadImage(settings.imagePath); // 加载图片
            //ApplyDisplayMode(settings.mode); // 应用显示模式
            ApplyBackgroundImage();
        }
    }

    [System.Serializable]
    public class BackgroundSettings
    {
        public string imagePath;
        public BackgroundImageMode mode;
    }

    public enum BackgroundImageMode
    {
        Tile,
        Fill
        
    }
}
