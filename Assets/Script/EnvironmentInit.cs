/* Module name: EnvironmentInit
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
using System.Diagnostics;
using UnityEngine;

public class EnvironmentInit : MonoBehaviour
{
    // Start is called before the first frame update
    private Process exeProcess;

    void Start()
    {
        StartExe();
    }

    // 启动 exe 的方法
    public void StartExe()
    {
        string exePath = Application.streamingAssetsPath + "/AI-Vtuber-LLM-Service.exe";// LLM服务程序路径

        UnityEngine.Debug.LogError(exePath);
        // 创建一个新的进程信息
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            CreateNoWindow = true// 确保exe挂在后台，没有窗口
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
        // 获取指定名称的所有进程
        Process[] processes = Process.GetProcessesByName("AI-Vtuber-LLM-Service");

        foreach (Process process in processes)
        {
            try
            {
                // 终止进程
                process.Kill();
                process.WaitForExit(); // 等待进程完全退出
                UnityEngine.Debug.Log($"Terminated process: {process.ProcessName} (ID: {process.Id})");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to terminate process: {process.ProcessName} (ID: {process.Id}). Error: {ex.Message}");
            }
        }
    }

    private void OnDestroy()
    {
        if (exeProcess != null && !exeProcess.HasExited)
        {
            exeProcess.Kill(); // 结束进程
            exeProcess.Dispose();
        }
    }

}
