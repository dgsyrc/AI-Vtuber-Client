using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class RealTimeDropdownUpdater : MonoBehaviour
{
    public TMP_Dropdown dropdown;  // ���� TMP_Dropdown ���
    public string targetPath;      // Ŀ��·��
    public float updateInterval = 5f;  // ���¼��ʱ�䣨�룩

    private float nextUpdateTime;

    void Start()
    {
        nextUpdateTime = Time.time + updateInterval;
        UpdateDropdown();
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateDropdown();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    void UpdateDropdown()
    {
        dropdown.ClearOptions();  // ������е�ѡ��

        if (Directory.Exists(targetPath))
        {
            string[] directories = Directory.GetDirectories(targetPath);  // ��ȡ�������ļ���
            var options = new List<TMP_Dropdown.OptionData>();

            foreach (var dir in directories)
            {
                string folderName = Path.GetFileName(dir);  // ��ȡ�ļ�������
                options.Add(new TMP_Dropdown.OptionData(folderName));  // ���ļ���������ӵ�ѡ����
            }

            dropdown.AddOptions(options);  // ��ѡ����ӵ� TMP_Dropdown ��
        }
        else
        {
            Debug.LogError("Ŀ��·��������!");
        }
    }
}
