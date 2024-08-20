using UnityEngine;
using UnityEngine.UI;

public class BackgroundModeSetter : MonoBehaviour
{
    public Image backgroundImage; // ����ͼ���
    private Sprite loadedSprite;

    public void SetBackgroundImage(Sprite sprite)
    {
        loadedSprite = sprite;
        backgroundImage.sprite = sprite;
        backgroundImage.color = Color.white; // ȷ������ɫΪ��ɫ
    }

    public void ApplyDisplayMode(BackgroundImageMode mode)
    {
        switch (mode)
        {
            case BackgroundImageMode.Tile:
                // ���� Image ���Ϊƽ��ģʽ
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = true;
                backgroundImage.material = null; // ȷ��û�в���Ӱ��
                break;

            case BackgroundImageMode.Fill:
                // ���� Image ���Ϊ���ģʽ  
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = false;
                backgroundImage.material = new Material(Shader.Find("Unlit/Texture"));
                backgroundImage.material.mainTexture = loadedSprite.texture;
                backgroundImage.sprite = null; // ȷ����ʹ�� Sprite
                break;
        }
    }

    public enum BackgroundImageMode
    {
        Tile,  // ƽ��ģʽ
        Fill // ���ģʽ
        
    }
}
