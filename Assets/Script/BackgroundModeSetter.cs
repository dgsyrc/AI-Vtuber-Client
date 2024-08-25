using UnityEngine;
using UnityEngine.UI;

public class BackgroundModeSetter : MonoBehaviour
{
    public Image backgroundImage; // ����ͼ���
    private Sprite loadedSprite;
    private Material loadedMaterial;
    public ImageManager imageManager;
    public void SetBackgroundImage(Sprite sprite)
    {
        loadedSprite = sprite;
        backgroundImage.sprite = sprite;
        loadedMaterial = new Material(Shader.Find("Unlit/Texture"));
        loadedMaterial.mainTexture = loadedSprite.texture;
        backgroundImage.color = Color.white; // ȷ������ɫΪ��ɫ
    }

    public void ApplyDisplayMode(int mode)
    {
        switch (mode)
        {
            case 0:
                // ���� Image ���Ϊƽ��ģʽ
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = true;
                backgroundImage.material = null; // ȷ��û�в���Ӱ��
                backgroundImage.sprite = loadedSprite;
                break;

            case 1:
                // ���� Image ���Ϊ���ģʽ  
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = false;
                backgroundImage.material = loadedMaterial;
                backgroundImage.material.mainTexture = loadedMaterial.mainTexture;
                backgroundImage.sprite = null; // ȷ����ʹ�� Sprite
                break;
        }
        imageManager.nowBackgroundData.mode = mode;
    }
}
