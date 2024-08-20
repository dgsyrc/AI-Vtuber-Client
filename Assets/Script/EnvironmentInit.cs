using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class EnvironmentInit : MonoBehaviour
{
    // Start is called before the first frame update
    private Process exeProcess;

    void Start()
    {
        StartExe();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // 启动 exe 的方法
    public void StartExe()
    {
        string exePath = Application.streamingAssetsPath + "/AI-Vtuber-LLM-Service.exe";//Path.Combine(Application.streamingAssetsPath, "AI-Vtuber-LLM-Service.exe");

        UnityEngine.Debug.LogError(exePath);
        // 创建一个新的进程信息
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath, 
            //CreateNoWindow = false // 确保exe挂在后台，没有窗口
        };

        // 启动进程
        exeProcess = Process.Start(startInfo);
        UnityEngine.Debug.LogError("Start");
    }

    // 游戏关闭时调用的方法，确保进程关闭
    private void OnApplicationQuit()
    {
        if (exeProcess != null && !exeProcess.HasExited)
        {
            exeProcess.Kill(); // 结束进程
            exeProcess.Dispose();
        }
    }
    
}
