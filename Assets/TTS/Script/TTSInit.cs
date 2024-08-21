using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TTSInit : MonoBehaviour
{
    // �󶨵�Inspector�е�Button

    // �������ļ���·��
    private string batchFilePath = @"\GPT-SoVITS-Inference\0 һ�������ű�\5 ������˳���.bat";

    // ���ڴ洢������bat�ļ��Ľ���
    private Process batProcess;

    void Start()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string projectPath = Directory.GetCurrentDirectory();//��ȡ��ǰUnity����Ŀ¼
            batchFilePath = projectPath + batchFilePath;
            startInfo.FileName = batchFilePath;
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(batchFilePath); // ���ù���Ŀ¼
            startInfo.UseShellExecute = false; // ��������Ϊ false �������ش��� ���ش�������
            startInfo.CreateNoWindow = true; // ����������

            // �����������ļ�
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
        // �ر�Ӧ�ó���ʱ��ֹbat�ļ��Ľ���
        if (batProcess != null && !batProcess.HasExited)
        {
            batProcess.Kill();
            UnityEngine.Debug.Log("Batch file process terminated.");
        }
    }
}
