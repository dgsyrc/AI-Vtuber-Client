using UnityEngine;
using UnityEngine.UI;

public class BackgroundModeSetter : MonoBehaviour
{
    public Image backgroundImage; // 背景图组件
    private Sprite loadedSprite;

    public void SetBackgroundImage(Sprite sprite)
    {
        loadedSprite = sprite;
        backgroundImage.sprite = sprite;
        backgroundImage.color = Color.white; // 确保背景色为白色
    }

    public void ApplyDisplayMode(BackgroundImageMode mode)
    {
        switch (mode)
        {
            case BackgroundImageMode.Tile:
                // 设置 Image 组件为平铺模式
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = true;
                backgroundImage.material = null; // 确保没有材质影响
                break;

            case BackgroundImageMode.Fill:
                // 设置 Image 组件为填充模式  
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.preserveAspect = false;
                backgroundImage.material = new Material(Shader.Find("Unlit/Texture"));
                backgroundImage.material.mainTexture = loadedSprite.texture;
                backgroundImage.sprite = null; // 确保不使用 Sprite
                break;
        }
    }

    public enum BackgroundImageMode
    {
        Tile,  // 平铺模式
        Fill // 填充模式
        
    }
}
