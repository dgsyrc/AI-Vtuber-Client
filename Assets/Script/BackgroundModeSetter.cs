using UnityEngine;
using UnityEngine.UI;

public class BackgroundModeSetter : MonoBehaviour
{
    public Image backgroundImage; // 背景图组件
    private Sprite loadedSprite;
    private Material loadedMaterial;
    public ImageManager imageManager;
    public void SetBackgroundImage(Sprite sprite)
    {
        loadedSprite = sprite;
        backgroundImage.sprite = sprite;
        loadedMaterial = new Material(Shader.Find("Unlit/Texture"));
        loadedMaterial.mainTexture = loadedSprite.texture;
        backgroundImage.color = Color.white; // 确保背景色为白色
    }

    public void ApplyDisplayMode(int mode)
    {
        switch (mode)
        {
            case 0:
                // 设置 Image 组件为平铺模式
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = true;
                backgroundImage.material = null; // 确保没有材质影响
                backgroundImage.sprite = loadedSprite;
                break;

            case 1:
                // 设置 Image 组件为填充模式  
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = false;
                backgroundImage.material = loadedMaterial;
                backgroundImage.material.mainTexture = loadedMaterial.mainTexture;
                backgroundImage.sprite = null; // 确保不使用 Sprite
                break;
        }
        imageManager.nowBackgroundData.mode = mode;
    }
}
