using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TTSInit : MonoBehaviour
{
    // 绑定到Inspector中的Button

    // 批处理文件的路径
    private string batchFilePath = @"\GPT-SoVITS-Inference\0 一键启动脚本\5 启动后端程序.bat";

    // 用于存储启动的bat文件的进程
    private Process batProcess;

    void Start()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string projectPath = Directory.GetCurrentDirectory();//获取当前Unity工作目录
            batchFilePath = projectPath + batchFilePath;
            startInfo.FileName = batchFilePath;
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(batchFilePath); // 设置工作目录
            startInfo.UseShellExecute = false; // 必须设置为 false 才能隐藏窗口 隐藏窗口设置
            startInfo.CreateNoWindow = true; // 不创建窗口

            // 启动批处理文件
            batProcess = Process.Start(startInfo);

            UnityEngine.Debug.Log("Batch file started.");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to start batch file: {ex.Message}");
        }
    }

    void OnApplicationQuit()
    {
        // 关闭应用程序时终止bat文件的进程
        if (batProcess != null && !batProcess.HasExited)
        {
            batProcess.Kill();
            UnityEngine.Debug.Log("Batch file process terminated.");
        }
    }
}
