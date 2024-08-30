/* Module name: SettingSave
 * Author: dgsyrc@github.com
 * Update date: 2024/08/30
 */
using UnityEngine;
using TMPro;

public class SettingSave : MonoBehaviour
{
    public TMP_InputField WindowWidthInputText;
    public TMP_InputField WindowHeightInputText;

    int lastWidth;
    int lastHeight;

    public void Start()
    {
        UpdateInputFieldsWithCurrentWindowSize();
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
    public void Save()
    {
        int width = int.Parse(WindowWidthInputText.text);
        int height = int.Parse(WindowHeightInputText.text);

        Screen.SetResolution(width, height, FullScreenMode.Windowed);
    }

    void Update()
    {
        // 检测窗口大小是否发生变化
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            // 如果发生变化，更新输入框内容
            UpdateInputFieldsWithCurrentWindowSize();

            // 记录当前窗口大小
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }
    void UpdateInputFieldsWithCurrentWindowSize()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        // 更新输入框的文本
        WindowWidthInputText.text = currentWidth.ToString();
        WindowHeightInputText.text = currentHeight.ToString();
    }
}
