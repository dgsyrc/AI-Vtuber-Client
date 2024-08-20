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
    // ���� exe �ķ���
    public void StartExe()
    {
        string exePath = Application.streamingAssetsPath + "/AI-Vtuber-LLM-Service.exe";//Path.Combine(Application.streamingAssetsPath, "AI-Vtuber-LLM-Service.exe");

        UnityEngine.Debug.LogError(exePath);
        // ����һ���µĽ�����Ϣ
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = exePath, 
            //CreateNoWindow = false // ȷ��exe���ں�̨��û�д���
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
    }
    
}
