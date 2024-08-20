using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class LayoutSaveProcessor : MonoBehaviour
{
    public class PanelData
    {
        //public string category;  // 用于分类，例如character或Danmaku
        public string name;      // Panel的名称，例如Rice或Custom
        public string show_name;
        //public Vector2 anchoredPosition;
        public float x;
        public float y;
        public float width;
        public float height;
        public float scaleX;
        public float scaleY;
        public float scaleZ;
        
    }
    // 保存所有数据的字典结构
    static private Dictionary<string, List<PanelData>> panelDataDictionary = new Dictionary<string, List<PanelData>>();

    // 保存数据
    static public void SavePanels()
    {
        // 假设我们有两个panel
        RectTransform panel1 = GameObject.Find("CharacterPanel-2").GetComponent<RectTransform>();
        RectTransform panel2 = GameObject.Find("DanmakuPanel").GetComponent<RectTransform>();

        Debug.LogError("Data find");
        // 创建panel1数据
        PanelData panel1Data = new PanelData
        {
            //category = "Panel",
            name = "Character",
            show_name = "人物",
            //anchoredPosition = panel1.anchoredPosition,
            x = panel1.localPosition.x,
            y = panel1.localPosition.y,
            width = panel1.rect.width,
            height = panel1.rect.height,
            scaleX = panel1.localScale.x,
            scaleY = panel1.localScale.y,
            scaleZ = panel1.localScale.z
        };

        // 创建panel2数据
        PanelData panel2Data = new PanelData
        {
            //category = "Panel",
            name = "Danmaku",
            show_name = "弹幕",
            //anchoredPosition = panel2.anchoredPosition,
            x = panel2.localPosition.x,
            y = panel2.localPosition.y,
            width = panel2.rect.width,
            height = panel2.rect.height,
            scaleX = panel2.localScale.x,
            scaleY = panel2.localScale.y,
            scaleZ = panel2.localScale.z
        };

        // 将panel1和panel2数据添加到字典
        AddPanelDataToDictionary(panel1Data,"Panel");
        AddPanelDataToDictionary(panel2Data,"Panel");

        // 将数据序列化为JSON并保存
        File.WriteAllText(Application.persistentDataPath + "/layout.json", string.Empty);
        string json = JsonConvert.SerializeObject(panelDataDictionary, Formatting.Indented);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/layout.json", json);

        Debug.Log("Data saved to: " + Application.persistentDataPath + "/layout.json");
    }

    // 向字典中添加数据
    static private void AddPanelDataToDictionary(PanelData panelData,string category)
    {
        // 检查是否存在该分类，如果不存在则创建
        if (!panelDataDictionary.ContainsKey(category))
        {
            panelDataDictionary[category] = new List<PanelData>();
        }

        // 添加该Panel的数据
        panelDataDictionary[category].Add(panelData);
    }

    // 加载数据
    public void LoadPanels()
    {
        string path = Application.persistentDataPath + "/panelData.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            panelDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<PanelData>>>(json);

            // 根据字典中的数据来设置Panel的RectTransform
            foreach (var category in panelDataDictionary)
            {
                foreach (var panelData in category.Value)
                {
                    RectTransform panel = GameObject.Find(panelData.name).GetComponent<RectTransform>();
                    //panel.anchoredPosition = panelData.anchoredPosition;
                    //panel.sizeDelta = panelData.sizeDelta;
                    //panel.localScale = panelData.scale;
                }
            }

            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogError("Save file not found.");
        }
    }
}
