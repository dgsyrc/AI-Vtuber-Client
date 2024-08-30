/* Module name: BackgroundImageManager
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ���� TextMeshPro �������ռ�
using UnityEngine.UI;
using System.IO;
using SFB; // ���� Standalone File Browser �������ռ�

public class BackgroundImageManager : MonoBehaviour
{
    public Button selectFileButton; // ѡ���ļ���ť��TMP_Button��
    public TMP_Text filePathText; // ��ʾ�ļ�·�����ı���TMP_Text��
    public TMP_Dropdown modeDropdown; // ��ʾģʽ�����˵���TMP_Dropdown��
    public Button applyButton; // Ӧ�ð�ť��TMP_Button��
    public Image backgroundImage; // ����ͼ���
    public string savePath; // ����·��
    public BackgroundModeSetter backgroundModeSetter; // ����ģʽ������
    public ImageManager imageManager;

    private Sprite loadedSprite;

    void Start()
    {
        // ���������˵���ѡ��
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(new List<string> { "ƽ��", "���" });

        // �󶨰�ť�¼�
        selectFileButton.onClick.AddListener(OpenFileDialog);
        applyButton.onClick.AddListener(ApplyBackgroundImage);

        // ���ر��������
        LoadSettings();
    }

    void OpenFileDialog()
    {
        // ���ļ�ѡ�񴰿�
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
        // Ӧ����ʾģʽ
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
            LoadImage(imageManager.nowBackgroundData.path); // ����ͼƬ
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
