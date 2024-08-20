using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class RealTimeDropdownUpdater : MonoBehaviour
{
    public TMP_Dropdown dropdown;  // 连接 TMP_Dropdown 组件
    public string targetPath;      // 目标路径
    public float updateInterval = 5f;  // 更新间隔时间（秒）

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
        dropdown.ClearOptions();  // 清除现有的选项

        if (Directory.Exists(targetPath))
        {
            string[] directories = Directory.GetDirectories(targetPath);  // 获取所有子文件夹
            var options = new List<TMP_Dropdown.OptionData>();

            foreach (var dir in directories)
            {
                string folderName = Path.GetFileName(dir);  // 获取文件夹名称
                options.Add(new TMP_Dropdown.OptionData(folderName));  // 将文件夹名称添加到选项中
            }

            dropdown.AddOptions(options);  // 将选项添加到 TMP_Dropdown 中
        }
        else
        {
            Debug.LogError("目标路径不存在!");
        }
    }
}
