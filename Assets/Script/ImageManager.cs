using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using SFB;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.U2D;

public class ImageManager : MonoBehaviour
{
    public GameObject imagePrefab; // Drag your Image prefab here
    public Transform imageContainer; // The Panel where images will be placed
    public Button addButton; // The button to add new images
    public Image backgroundImage;
    public TMP_Text backgroundImagePathText;
    public RectTransform L2DPanel;
    public RectTransform askPanel;
    public RectTransform danmakuPanel;
    public TMP_Dropdown modeDropdown;

    private List<GameObject> images = new List<GameObject>();
    public BackgroundData nowBackgroundData = new BackgroundData();

    public BackgroundImageManager backgroundImageManager;

    void Start()
    {
        addButton.onClick.AddListener(OpenFileDialog);
        LoadImages();
    }
    public static GameObject Instantiate(GameObject prefab, Transform parent, Sprite img, string path)
    {
        GameObject instance = Instantiate(prefab, parent);
        instance.GetComponent<Image>().sprite = img;
        instance.GetComponent<RectTransform>().sizeDelta = new Vector2 (img.rect.width, img.rect.height);
        instance.name = path;
        return instance;
    }
    void AddImage(Sprite loadSprite, string path)
    {
        GameObject newImage = Instantiate(imagePrefab, imageContainer, loadSprite, path);
        //RectTransform rectTransform = newImage.GetComponent<RectTransform>();
        images.Add(newImage);
    }

    public void SaveImages()
    {
        List<ImageData> imageDataList = new List<ImageData>();
        L2DData l2DDataTmp = new L2DData();
        DanmakuData danmakuDataTmp = new DanmakuData();
        AskData askDataTmp = new AskData();
        foreach (var img in images)
        {
            if(img != null)
            {
                ImageData data = new ImageData
                {
                    position = img.GetComponent<RectTransform>().anchoredPosition,
                    size = img.GetComponent<RectTransform>().sizeDelta,
                    scale = img.GetComponent<RectTransform>().localScale,
                    path = img.name
                };
                imageDataList.Add(data);
            }
        }
        l2DDataTmp.position = L2DPanel.anchoredPosition;
        l2DDataTmp.size = L2DPanel.sizeDelta;
        l2DDataTmp.scale = L2DPanel.localScale;
        danmakuDataTmp.position = danmakuPanel.anchoredPosition;
        danmakuDataTmp.size = danmakuPanel.sizeDelta;
        danmakuDataTmp.scale = danmakuPanel.localScale;
        askDataTmp.position = askPanel.anchoredPosition;
        askDataTmp.size = askPanel.sizeDelta;
        askDataTmp.scale = askPanel.localScale;
       
        ImageList tmpImage = new ImageList { images = imageDataList ,backgroundData = nowBackgroundData, askData = askDataTmp, danmakuData=danmakuDataTmp,l2DData=l2DDataTmp};
        ImageConfig tmpConfig = new ImageConfig { imageData = tmpImage };
        //tmpConfig.backgroundData.path = nowBackgroundData.path;
        //tmpConfig.backgroundData.mode = nowBackgroundData.mode;
        //UnityEngine.Debug.LogError("path:" + tmpConfig.backgroundData.path+" mode:" + tmpConfig.backgroundData.mode.ToString());
        //string json = JsonConvert.SerializeObject(tmpConfig, Formatting.Indented);
        string json = JsonUtility.ToJson(tmpConfig);
        File.WriteAllText(UnityEngine.Application.persistentDataPath + "/imageData.json", json);
    }

    void LoadImages()
    {
        string path = UnityEngine.Application.persistentDataPath + "/imageData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ImageConfig imageConfig = JsonUtility.FromJson<ImageConfig>(json);
            ImageList imageList = imageConfig.imageData;
            BackgroundData backgroundSetting = imageConfig.imageData.backgroundData;
            L2DData l2DDataTmp = imageConfig.imageData.l2DData;
            DanmakuData danmakuDataTmp = imageConfig.imageData.danmakuData;
            AskData askDataTmp = imageConfig.imageData.askData;
            nowBackgroundData = backgroundSetting;

            foreach (var data in imageList.images)
            {
                GameObject newImage = Instantiate(imagePrefab, imageContainer);
                RectTransform rectTransform = newImage.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = data.position;
                rectTransform.sizeDelta = data.size;
                rectTransform.localScale = data.scale;
                newImage.name = data.path;
                UnityEngine.Debug.LogError("File Read: "+newImage.name);
                if (File.Exists(newImage.name))
                {
                    byte[] fileData = File.ReadAllBytes(newImage.name);
                    Texture2D texture = new Texture2D(2, 2);
                    Sprite loadedSprite;
                    texture.LoadImage(fileData);
                    loadedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    newImage.GetComponent<Image>().sprite = loadedSprite;
                    newImage.GetComponent<RectTransform>().sizeDelta = new Vector2(loadedSprite.rect.width, loadedSprite.rect.height);
                    images.Add(newImage);
                }
                else
                {
                    Debug.LogError("File not found: " + path);
                    continue;
                }
                
            }
            
            /*if (File.Exists(backgroundSetting.path))
            {
                byte[] fileData = File.ReadAllBytes(backgroundSetting.path);
                Texture2D texture = new Texture2D(2, 2);
                Sprite loadedSprite;
                texture.LoadImage(fileData);
                loadedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                backgroundImage.sprite = loadedSprite;
                backgroundImage.color = Color.white; // 确保背景色为白色
                UnityEngine.Debug.LogError("path1:" + backgroundSetting.path);
                switch (backgroundSetting.mode)
                {
                     case 0:
                         // 设置 Image 组件为平铺模式
                         backgroundImage.type = Image.Type.Simple;
                         backgroundImage.preserveAspect = true;
                         backgroundImage.material = null; // 确保没有材质影响
                         break;

                     case 1:
                         // 设置 Image 组件为填充模式  
                         backgroundImage.type = Image.Type.Simple;
                         backgroundImage.preserveAspect = false;
                         backgroundImage.material = new Material(Shader.Find("Unlit/Texture"));
                         backgroundImage.material.mainTexture = loadedSprite.texture;
                         backgroundImage.sprite = null; // 确保不使用 Sprite
                         break;
                }
                modeDropdown.value = backgroundSetting.mode;
                nowBackgroundData = backgroundSetting;
                backgroundImagePathText.text = backgroundSetting.path;
                UnityEngine.Debug.LogError("path1:" + backgroundSetting.path);
            }
            else
            {
                Debug.LogError("File not found: " + path);
            }
            Debug.LogError("File show: " + backgroundImagePathText.text);
            backgroundImagePathText.text = backgroundSetting.path;*/
            L2DPanel.anchoredPosition = l2DDataTmp.position;
            L2DPanel.sizeDelta = l2DDataTmp.size;
            L2DPanel.localScale = l2DDataTmp.scale;
            danmakuPanel.anchoredPosition = danmakuDataTmp.position;
            danmakuPanel.sizeDelta = danmakuDataTmp.size;
            danmakuPanel.localScale = danmakuDataTmp.scale;
            askPanel.anchoredPosition = askDataTmp.position;
            askPanel.sizeDelta = askDataTmp.size;
            askPanel.localScale = askDataTmp.scale;
           
            backgroundImageManager.LoadSettings();
            Debug.LogError("File show: " + backgroundSetting.path);
        }
    }

    void OpenFileDialog()
    {
        // 打开文件选择窗口
        var extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);

        if (paths.Length > 0)
        {
            string path = paths[0];
            LoadImage(path);
        }
    }

    void LoadImage(string path)
    {
        if (File.Exists(path))
        {
            Sprite loadedSprite;
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            loadedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            AddImage(loadedSprite,path);
            //backgroundImage.sprite = loadedSprite;
            //backgroundImage.color = Color.white;
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }
}

[System.Serializable]
public class ImageData
{
    public Vector2 position;
    public Vector2 size;
    public Vector3 scale;
    public string path;
}

[System.Serializable]
public class BackgroundData
{
    public string path;
    public int mode;
}

[System.Serializable]
public class L2DData
{
    public Vector2 position;
    public Vector2 size;
    public Vector3 scale;
}

[System.Serializable]
public class DanmakuData
{
    public Vector2 position;
    public Vector2 size;
    public Vector3 scale;
}

[System.Serializable]
public class AskData
{
    public Vector2 position;
    public Vector2 size;
    public Vector3 scale;
}


[System.Serializable]
public class ImageList
{
    public List<ImageData> images;
    public BackgroundData backgroundData;
    public L2DData l2DData;
    public DanmakuData danmakuData;
    public AskData askData;
}

[System.Serializable]

public class ImageConfig
{
    public ImageList imageData;
}