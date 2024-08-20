using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCapture : MonoBehaviour
{
    public Canvas canvas;
    public RenderTexture renderTexture;

    private void Start()
    {
        CaptureCanvas();
    }

    void CaptureCanvas()
    {
        // 创建一个新的 RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        // 设置画布的渲染目标
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.GetComponent<Camera>().targetTexture = renderTexture;
    }
}
