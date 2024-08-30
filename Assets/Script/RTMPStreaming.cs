/* Module name: RTMPStreaming
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using TMPro;
public class RTMPStreaming : MonoBehaviour
{
    public Camera cameraToRecord;
    public RenderTexture renderTexture;
    public string rtmpUrl = "rtmp://live-push.bilivideo.com/live-bvc/?streamname=";
    public string tmpVideo;
    public int width = 1920;
    public int height = 1080;
    public int sampleRate = 44100;
    public int channels = 2;
    public float framerate = 30f;
    public TMP_InputField rtmpInput;
  
    private Process ffmpegProcess;
    private string ffmpegPath;
    private bool isRecording = false;
    private DateTime timenow;
    private Stream ffmpegInputStream;
    // Start is called before the first frame update
    void Start()
    {
        timenow = DateTime.Now;
        ffmpegPath = UnityEngine.Application.streamingAssetsPath + "/ffmpeg.exe";
        tmpVideo = UnityEngine.Application.persistentDataPath + "/rec" + timenow.Ticks.ToString() + ".flv";

        renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        // 启动音频和视频录制进程
        StartStreaming();
    }

    public void StartStreaming()
    {
        //string videoPipe = @"\\.\pipe\video_pipe";
        //string audioPipe = @"\\.\pipe\audio_pipe";
        string tmpVideo = UnityEngine.Application.persistentDataPath + "/rec.flv";
        // 构造 FFmpeg 命令行参数
        rtmpUrl = rtmpInput.text;
        string ffmpegArgs =
            $"-re -f rawvideo -pix_fmt rgba -s {width}x{height} -i - " +  // 输入视频
            $"-f dshow -i audio=\"virtual-audio-capturer\" " +       // 输入音频
            $"-af \"volume=1.5\" -vf \"vflip\" -vcodec libx264 -preset:v ultrafast -pix_fmt yuv420p -acodec aac -b:a 529200 -aac_coder fast -profile:a aac_low -cutoff 22050 -ac 2 -ar 44100 " +                         // 视频编码配置                                       
            $"-f mpegts -f flv {rtmpUrl}";
        ffmpegProcess = new Process();
        ffmpegProcess.StartInfo.FileName = ffmpegPath;
        ffmpegProcess.StartInfo.Arguments = ffmpegArgs;
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.RedirectStandardError = true;
        ffmpegProcess.StartInfo.RedirectStandardOutput = true;
        ffmpegProcess.StartInfo.CreateNoWindow = true;
        ffmpegProcess.StartInfo.RedirectStandardInput = true;
        ffmpegProcess.Start();
        ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;
        isRecording = true;
        UnityEngine.Debug.LogError("In stream");
        ffmpegProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log($"[FFmpeg Output] {args.Data}");
        ffmpegProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError($"[FFmpeg Error] {args.Data}");
        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.BeginOutputReadLine();
        StartCoroutine(CaptureFrames());
        
    }
    IEnumerator CaptureFrames()
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        UnityEngine.Debug.LogError("frame rec status "+isRecording.ToString());
        while (true)
        {
            if (isRecording)
            {
                UnityEngine.Debug.LogError("IN PROCESS");
                RenderTexture previousRenderTexture = cameraToRecord.targetTexture;
                SetCullingMask("character");
                cameraToRecord.targetTexture = renderTexture;
                RenderTexture.active = renderTexture;
                cameraToRecord.Render();
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                //RenderTexture.active = null;
                SetCullingMask("Everything");
                cameraToRecord.targetTexture = previousRenderTexture;
                cameraToRecord.Render();
                // Get the raw data from the texture
                byte[] bytes = texture.GetRawTextureData(); // Use GetRawTextureData() for raw RGBA data

                try
                {
                     ffmpegInputStream.Write(bytes, 0, bytes.Length);
                     ffmpegInputStream.Flush();
                }
                catch (IOException e)
                {
                    UnityEngine.Debug.LogError($"Failed to write to ffmpeg input stream: {e.Message}");
                }
            }
            yield return new WaitForSeconds(1f / framerate);

        }
    }
    void OnApplicationQuit()
    {
        isRecording = false;
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            ffmpegProcess.Kill();
        }
    }
    void SetCullingMask(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layerName == "Everything")
        {
            cameraToRecord.cullingMask = -1;
        }
        else
        {
            // 设置剔除遮罩
            cameraToRecord.cullingMask = 1 << layer;
        }
        return;
    }
}
