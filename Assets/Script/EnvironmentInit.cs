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

    // ���� exe �ķ���
    public void StartExe()
    {
        string exePath = Application.streamingAssetsPath + "/AI-Vtuber-LLM-Service.exe";// LLM�������·��

        UnityEngine.Debug.LogError(exePath);
        // ����һ���µĽ�����Ϣ
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            CreateNoWindow = true// ȷ��exe���ں�̨��û�д���
        };

        // ��������
        exeProcess = Process.Start(startInfo);
        UnityEngine.Debug.LogError("Start");
    }

    // ��Ϸ�ر�ʱ���õķ�����ȷ�����̹ر�
    private void OnApplicationQuit()
    {
        if (exeProcess != null && !exeProcess.HasExited)
        {
            exeProcess.Kill(); // ��������
            exeProcess.Dispose();
        }
        // ��ȡָ�����Ƶ����н���
        Process[] processes = Process.GetProcessesByName("AI-Vtuber-LLM-Service");

        foreach (Process process in processes)
        {
            try
            {
                // ��ֹ����
                process.Kill();
                process.WaitForExit(); // �ȴ�������ȫ�˳�
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
            exeProcess.Kill(); // ��������
            exeProcess.Dispose();
        }
    }

}
