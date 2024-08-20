using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class StreamToRTMP : MonoBehaviour
{
    public Camera cameraToStream; // Assign the camera to stream in the inspector
    public RenderTexture renderTexture; // Assign a RenderTexture in the inspector
    public string rtmpUrl = "rtmp://live-push.bilivideo.com/live-bvc?streamname=live_179225431_2633288&key=844565e01ec65e1390fbf45ecc7c746e&schedule=rtmp&pflag=1"; // RTMP URL
    public int width = 1920;
    public int height = 1080;
    private Process ffmpegProcess;
    void Start()
    {
        renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        cameraToStream.targetTexture = renderTexture;
        //cameraToStream.targetDisplay = 0; // ÉèÖÃÎª Display 1
        StartStreaming();
    }

    void OnApplicationQuit()
    {
        StopStreaming();
    }

    public void StartStreaming()
    {
        if (cameraToStream == null || renderTexture == null)
        {
            UnityEngine.Debug.LogError("Camera or RenderTexture not assigned.");
            return;
        }

        // Ensure RenderTexture is set up for the camera
        cameraToStream.targetTexture = renderTexture;

        // Path to ffmpeg executable
        string ffmpegPath = UnityEngine.Application.streamingAssetsPath + "/ffmpeg.exe";

        // Create and configure the ffmpeg process
        ffmpegProcess = new Process();
        ffmpegProcess.StartInfo.FileName = ffmpegPath;
        ffmpegProcess.StartInfo.Arguments = $"-f rawvideo -pix_fmt rgba -s {width}x{height} -i - -vf \"vflip\" -c:v libx264 -pix_fmt yuv420p -b:v 1M -f flv {rtmpUrl}";
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.RedirectStandardInput = true;
        ffmpegProcess.StartInfo.RedirectStandardOutput = true;
        ffmpegProcess.StartInfo.RedirectStandardError = true;
        ffmpegProcess.StartInfo.CreateNoWindow = true;
        ffmpegProcess.Start();

        // Set up handlers for ffmpeg output
        ffmpegProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log($"[FFmpeg Output] {args.Data}");
        ffmpegProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError($"[FFmpeg Error] {args.Data}");

        // Start reading from ffmpeg output and error streams
        ffmpegProcess.BeginOutputReadLine();
        ffmpegProcess.BeginErrorReadLine();

        // Get the stream for sending data to ffmpeg
        Stream ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;

        // Capture and send RenderTexture data to ffmpeg
        StartCoroutine(CaptureAndStream(ffmpegInputStream));
    }

    public void StopStreaming()
    {
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            ffmpegProcess.StandardInput.Close();
            ffmpegProcess.WaitForExit();
            ffmpegProcess.Close();
            ffmpegProcess = null;
            cameraToStream.targetTexture = null;
        }
    }

    private IEnumerator CaptureAndStream(Stream ffmpegInputStream)
    {
        // Create a temporary texture to read from RenderTexture
        Texture2D tempTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        while (true)
        {
            // Capture the current frame
            cameraToStream.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            tempTexture.ReadPixels(new Rect(0, 0, width,height), 0, 0);
            RenderTexture.active = null;

            // Get the raw data from the texture
            byte[] bytes = tempTexture.GetRawTextureData(); // Use GetRawTextureData() for raw RGBA data

            // Write the data to ffmpeg's input stream
            try
            {
                ffmpegInputStream.Write(bytes, 0, bytes.Length);
                ffmpegInputStream.Flush();
            }
            catch (IOException e)
            {
                UnityEngine.Debug.LogError($"Failed to write to ffmpeg input stream: {e.Message}");
                break;
            }
            //cameraToStream.targetTexture = null;
            // Yield to wait for the next frame
            yield return new WaitForSeconds(1f / 30f); // Assuming 30 FPS
        }
        //cameraToStream.targetTexture = null;
    }
}
