/* Module name: BackgroundImageManager
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
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
    public ImageManager imageManager;

    private Sprite loadedSprite;

    void Start()
    {
        // 设置下拉菜单的选项
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
            imageManager.nowBackgroundData.path = path;
            ApplyBackgroundImage();
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }

    public void ApplyBackgroundImage()
    {
        // 应用显示模式
        //ApplyDisplayMode((BackgroundImageMode)modeDropdown.value);
        imageManager.nowBackgroundData.mode = modeDropdown.value;
        backgroundModeSetter.ApplyDisplayMode(modeDropdown.value);
    }

    public void LoadSettings()
    {
        if (File.Exists(imageManager.nowBackgroundData.path))
        {
            filePathText.text = imageManager.nowBackgroundData.path;
            modeDropdown.value = imageManager.nowBackgroundData.mode;
            LoadImage(imageManager.nowBackgroundData.path); // 加载图片
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
