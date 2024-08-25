using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using System.IO.Pipes;
using System.Threading.Tasks;
public class RTMPStreaming : MonoBehaviour
{
    public Camera cameraToRecord;
    //public AudioSource audioSource;
    public string rtmpUrl = "rtmp://live-push.bilivideo.com/live-bvc/?streamname=live_179225431_2633288&key=844565e01ec65e1390fbf45ecc7c746e&schedule=rtmp&pflag=1";
    public string tmpVideo;
    public int width = 1920;
    public int height = 1080;
    public float framerate = 30f;
    public int sampleRate = 44100;
    public int channels = 2;
    public RenderTexture renderTexture;

    private NamedPipeServerStream videoPipeServer;
    private NamedPipeServerStream audioPipeServer;
    private NamedPipeClientStream videoPipeClient;
    private NamedPipeClientStream audioPipeClient;
    private Process audioProcess;
    private Process videoProcess;
    private Process streamProcess;
    private Process ffmpegProcess;
    private string ffmpegPath;
    private string ffmpegPath_audio;
    private string ffmpegPath_video;
    private Stream ffmpegVideoInputStream;
    private Stream ffmpegAudioInputStream;
    private bool isRecording = false;
    private DateTime timenow;
    private Stream ffmpegInputStream;
    // Start is called before the first frame update
    void Start()
    {
        timenow = DateTime.Now;
        ffmpegPath = UnityEngine.Application.streamingAssetsPath + "/ffmpeg.exe";
        ffmpegPath_audio = UnityEngine.Application.streamingAssetsPath + "/ffmpeg_audio.exe";
        ffmpegPath_video = UnityEngine.Application.streamingAssetsPath + "/ffmpeg_video.exe";

        tmpVideo = UnityEngine.Application.persistentDataPath + "/rec" + timenow.Ticks.ToString() + ".flv";
        
        // 启动音频和视频录制进程
        StartRecording();
    }

    void StartRecording()
    {
        renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();
      
        //StartVideoRecording();
        //StartAudioRecording();
        StartStreaming();
    }

    void StartAudioRecording()
    {
        audioPipeServer = new NamedPipeServerStream("audio_pipe", PipeDirection.Out);
       // Task.Run(() => ConnectToPipeAsync(audioPipeServer));
    }

    void StartVideoRecording()
    {
        //videoPipeServer = new NamedPipeServerStream("video_pipe", PipeDirection.Out);
        //Task.Run(() => ConnectToPipeAsync(videoPipeServer));
        StartCoroutine(CaptureFrames());
    }

    void StartStreaming()
    {
        /* string streamArguments = $"ffmpeg -f rawvideo -pix_fmt rgba -s 1920x1080 -i \\\\.\\pipe\\video_pipe -f f32le -ar 44100 -ac 2 -i \\\\.\\pipe\\audio_pipe -c:v libx264 -pix_fmt yuv420p -b:v 1M -c:a aac -b:a 128k -f flv {rtmpUrl}";

         ProcessStartInfo streamStartInfo = new ProcessStartInfo
         {
             FileName = ffmpegPath,
             Arguments = streamArguments,
             UseShellExecute = false,
             RedirectStandardInput = true, // 来自音频和视频进程
             RedirectStandardOutput = true,
             RedirectStandardError = true,
             CreateNoWindow = false
         };*/
        
        //string videoPipe = @"\\.\pipe\video_pipe";
        //string audioPipe = @"\\.\pipe\audio_pipe";
        string tmpVideo = UnityEngine.Application.persistentDataPath + "/rec.flv";
        // 构造 FFmpeg 命令行参数
        /*string ffmpegArgs = 
            $"-re -f rawvideo -pix_fmt rgba -s {width}x{height} -i \\\\.\\pipe\\video_pipe " +  // 输入视频
            $"-f f32le -ar {sampleRate} -ac {channels} -i  \\\\.\\pipe\\audio_pipe " +       // 输入音频
            $"-vf \"vflip\" -c:v libx264 -pix_fmt yuv420p -b:v 1M " +                         // 视频编码配置
            $"-c:a aac -b:a 128k " +                                            // 音频编码配置
            $"-f flv {rtmpUrl}";        */
        string ffmpegArgs =
            $"-re -f rawvideo -pix_fmt rgba -s {width}x{height} -i - " +  // 输入视频
            $"-f dshow -i audio=\"virtual-audio-capturer\" " +       // 输入音频
            $"-af \"volume=1.5\" -vf \"vflip\" -vcodec libx264 -preset:v ultrafast -pix_fmt yuv420p -acodec aac -b:a 529200 -aac_coder fast -profile:a aac_low -cutoff 22050 -ac 2 -ar 44100 " +                         // 视频编码配置                                       
            $"-f mpegts -f flv {rtmpUrl}";
        //$"-f rawvideo -pix_fmt rgba -s {width}x{height} -i - -f dshow -i audio=" + "\"virtual-audio-capturer\"" + $" -c:v libx264 -pix_fmt yuv420p -b:v 1M -y {tmpVideo}";-f flv  -vcodec libx264 -preset:v ultrafast -pix_fmt yuv420p -acodec aac -f flv   $"-vf \"vflip\" -c:v libx264 -pix_fmt yuv420p -b:v 1M " +    $"-c:a aac -b:a 128k -f flv -y {tmpVideo}";
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
        // 将音频和视频进程的输出管道传递到混流进程的输入
        //audioProcess.StandardOutput.BaseStream.CopyToAsync(streamProcess.StandardInput.BaseStream);
        //videoProcess.StandardOutput.BaseStream.CopyToAsync(streamProcess.StandardInput.BaseStream);
    }
    IEnumerator CaptureFrames()
    {
        //RenderTexture renderTexture = new RenderTexture(width, height, 24);
        //cameraToRecord.targetTexture = renderTexture;
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

                //videoPipeServer.WriteAsync(bytes, 0, bytes.Length);
                //videoPipeServer.Flush();

                try
                {
                     ffmpegInputStream.Write(bytes, 0, bytes.Length);
                     ffmpegInputStream.Flush();
                }
                catch (IOException e)
                {
                    UnityEngine.Debug.LogError($"Failed to write to ffmpeg input stream: {e.Message}");
                    //break;
                }
                //videoPipeServer.Close();

            }
            yield return new WaitForSeconds(1f / framerate);

        }
        //yield return new WaitForSeconds(0.5f);
        //videoProcess.StandardInput.BaseStream.Close();
    }

   /* void OnAudioFilterRead(float[] data, int channels)
    {
        //Task.Run(() => ConnectToPipeAsync(audioPipeServer));
        UnityEngine.Debug.LogError("audio rec status " + isRecording.ToString());
        if (audioPipeServer != null && isRecording)
        {

            byte[] byteArray = new byte[data.Length * sizeof(float)];
            try
            {
                Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
                //Task.Run(() => ConnectToPipeAsync(audioPipeServer));
                audioPipeServer.WriteAsync(byteArray, 0, byteArray.Length);
                audioPipeServer.Flush();
            }
            catch (Exception e)
            {
                Task.Run(() => ConnectToPipeAsync(audioPipeServer));
                UnityEngine.Debug.LogError("EXPT");
            }
            
           // audioPipeServer.Flush();
           // audioPipeServer.Close();
           
        }
        //audioPipeServer.Close();
    }*/

   /* private async Task ConnectToPipeAsync(NamedPipeServerStream pipeServer)
    {
        try
        {
            await Task.Run(() => pipeServer.WaitForConnection());
            UnityEngine.Debug.Log("Pipe client connected.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to connect to pipe: {e.Message}");
        }
    }*/

    void OnApplicationQuit()
    {
        isRecording = false;
        //videoPipeServer.Close();
        //audioPipeServer.Close();
       /* if (audioProcess != null && !audioProcess.HasExited)
        {
            audioProcess.Kill();
        }

        if (videoProcess != null && !videoProcess.HasExited)
        {
            videoProcess.Kill();
        }

        if (streamProcess != null && !streamProcess.HasExited)
        {
            streamProcess.Kill();
        }*/
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
