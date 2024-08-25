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
        //savePath = Path.Combine(Application.persistentDataPath, "BackgroundSettings.json");
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
            //backgroundImage.sprite = loadedSprite;
            //backgroundImage.color = Color.white;
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

    /*void ApplyDisplayMode(BackgroundImageMode mode)
    {
        switch (mode)
        {
            case BackgroundImageMode.Fill:
                // ���� Image ���Ϊ���ģʽ
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = true;
                break;

            case BackgroundImageMode.Tile:
                // ���� Image ���Ϊƽ��ģʽ
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = false;
                backgroundImage.material = new Material(Shader.Find("Unlit/Texture"));
                backgroundImage.material.mainTexture = loadedSprite.texture;
                backgroundImage.sprite = null; // ȷ����ʹ�� Sprite
                break;
        }
    }*/
    public void LoadSettings()
    {
        if (File.Exists(imageManager.nowBackgroundData.path))
        {
            //string json = File.ReadAllText(imageManager.nowBackgroundData.path);
            //BackgroundSettings settings = JsonUtility.FromJson<BackgroundSettings>(json);
            filePathText.text = imageManager.nowBackgroundData.path;
            modeDropdown.value = imageManager.nowBackgroundData.mode;
            LoadImage(imageManager.nowBackgroundData.path); // ����ͼƬ
            //ApplyDisplayMode(settings.mode); // Ӧ����ʾģʽ
            //ApplyBackgroundImage();
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
