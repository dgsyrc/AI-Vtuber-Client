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
        // ��ⴰ�ڴ�С�Ƿ����仯
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            // ��������仯���������������
            UpdateInputFieldsWithCurrentWindowSize();

            // ��¼��ǰ���ڴ�С
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }
    void UpdateInputFieldsWithCurrentWindowSize()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        // �����������ı�
        WindowWidthInputText.text = currentWidth.ToString();
        WindowHeightInputText.text = currentHeight.ToString();
    }
}
