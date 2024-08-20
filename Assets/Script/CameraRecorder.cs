using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CameraRecorder : MonoBehaviour
{
    public Camera cameraToRecord;
    public int width = 1920;
    public int height = 1080;
    public string outputFilePath = "output.mp4";
    private RenderTexture renderTexture;
    private Process ffmpegProcess;
    private Stream ffmpegInputStream;

    void Start()
    {
        outputFilePath = Application.persistentDataPath + "/output.mp4";
        if (cameraToRecord == null)
        {
            UnityEngine.Debug.LogError("Camera is not assigned.");
            return;
        }

        // 创建 RenderTexture
        renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        cameraToRecord.targetTexture = renderTexture;


        StartFFMPEGRecord();
    }

    void StartFFMPEGRecord()
    {
        string ffmpegPath = Application.streamingAssetsPath + "/ffmpeg.exe";
        //string ffmpegPath = "ffmpeg"; // 确保 ffmpeg 在系统路径中
        string arguments = $"-f rawvideo -pix_fmt rgba -s {width}x{height} -i - -vf \"vflip\" -c:v libx264 -pix_fmt yuv420p -b:v 1M {outputFilePath}";

        ffmpegProcess = new Process();
        ffmpegProcess.StartInfo.FileName = ffmpegPath;
        ffmpegProcess.StartInfo.Arguments = arguments;
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.RedirectStandardInput = true;
        ffmpegProcess.StartInfo.RedirectStandardError = true;
        ffmpegProcess.StartInfo.CreateNoWindow = true;
        ffmpegProcess.Start();

        ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;

        // 读取错误流并输出到控制台
        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {    
                UnityEngine.Debug.LogError("FFmpeg Error: " + args.Data);
            }
        };

        // 定时将 RenderTexture 内容写入 ffmpeg 输入流
        InvokeRepeating("RecordRenderTexture", 0f, 1f / 30f); // 每秒 30 帧
    }

    void RecordRenderTexture()
    {
        if (cameraToRecord == null || renderTexture == null || ffmpegInputStream == null)
        {
            return;
        }

        try
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            byte[] bytes = texture.GetRawTextureData();

            // 分批次写入数据
            int chunkSize = 1024 * 1024; // 1MB
            for (int i = 0; i < bytes.Length; i += chunkSize)
            {
                int size = Mathf.Min(chunkSize, bytes.Length - i);
                ffmpegInputStream.Write(bytes, i, size);
            }

            RenderTexture.active = currentRT;
        }
        catch (IOException e)
        {
            UnityEngine.Debug.LogError($"IOException occurred: {e.Message}");
            CancelInvoke("RecordRenderTexture");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Exception occurred: {e.Message}");
            CancelInvoke("RecordRenderTexture");
        }
    }

    void OnDestroy()
    {
        // 停止录制
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            ffmpegProcess.StandardInput.Close();
            ffmpegProcess.WaitForExit();
            ffmpegProcess.Close();
        }

        // 清理 RenderTexture
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
