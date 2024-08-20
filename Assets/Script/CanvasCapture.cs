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
        // ����һ���µ� RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        // ���û�������ȾĿ��
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.GetComponent<Camera>().targetTexture = renderTexture;
    }
}
