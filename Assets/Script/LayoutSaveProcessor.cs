using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class LayoutSaveProcessor : MonoBehaviour
{
    public class PanelData
    {
        //public string category;  // ���ڷ��࣬����character��Danmaku
        public string name;      // Panel�����ƣ�����Rice��Custom
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
    // �����������ݵ��ֵ�ṹ
    static private Dictionary<string, List<PanelData>> panelDataDictionary = new Dictionary<string, List<PanelData>>();

    // ��������
    static public void SavePanels()
    {
        // ��������������panel
        RectTransform panel1 = GameObject.Find("CharacterPanel-2").GetComponent<RectTransform>();
        RectTransform panel2 = GameObject.Find("DanmakuPanel").GetComponent<RectTransform>();

        Debug.LogError("Data find");
        // ����panel1����
        PanelData panel1Data = new PanelData
        {
            //category = "Panel",
            name = "Character",
            show_name = "����",
            //anchoredPosition = panel1.anchoredPosition,
            x = panel1.localPosition.x,
            y = panel1.localPosition.y,
            width = panel1.rect.width,
            height = panel1.rect.height,
            scaleX = panel1.localScale.x,
            scaleY = panel1.localScale.y,
            scaleZ = panel1.localScale.z
        };

        // ����panel2����
        PanelData panel2Data = new PanelData
        {
            //category = "Panel",
            name = "Danmaku",
            show_name = "��Ļ",
            //anchoredPosition = panel2.anchoredPosition,
            x = panel2.localPosition.x,
            y = panel2.localPosition.y,
            width = panel2.rect.width,
            height = panel2.rect.height,
            scaleX = panel2.localScale.x,
            scaleY = panel2.localScale.y,
            scaleZ = panel2.localScale.z
        };

        // ��panel1��panel2������ӵ��ֵ�
        AddPanelDataToDictionary(panel1Data,"Panel");
        AddPanelDataToDictionary(panel2Data,"Panel");

        // ���������л�ΪJSON������
        File.WriteAllText(Application.persistentDataPath + "/layout.json", string.Empty);
        string json = JsonConvert.SerializeObject(panelDataDictionary, Formatting.Indented);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/layout.json", json);

        Debug.Log("Data saved to: " + Application.persistentDataPath + "/layout.json");
    }

    // ���ֵ����������
    static private void AddPanelDataToDictionary(PanelData panelData,string category)
    {
        // ����Ƿ���ڸ÷��࣬����������򴴽�
        if (!panelDataDictionary.ContainsKey(category))
        {
            panelDataDictionary[category] = new List<PanelData>();
        }

        // ��Ӹ�Panel������
        panelDataDictionary[category].Add(panelData);
    }

    // ��������
    public void LoadPanels()
    {
        string path = Application.persistentDataPath + "/panelData.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            panelDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<PanelData>>>(json);

            // �����ֵ��е�����������Panel��RectTransform
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
