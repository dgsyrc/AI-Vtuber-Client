using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class test01 : MonoBehaviour
{
    private UnityClient.AIInterface aiInterface;

    void Start()
    {
        UnityClient client = FindObjectOfType<UnityClient>();  // 获取UnityClient实例
        aiInterface = new UnityClient.AIInterface(client);  // 创建AIInterface实例
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))  // 例如按空格键设置新Prompt
        {
            aiInterface.SetPrompt("你好啊");
        }

        if (Input.GetKeyDown(KeyCode.Return))  // 例如按回车键获取AI回答
        {
            string answer = aiInterface.GetAnswer();
            Debug.Log("AI回答: " + answer);
        }
    }
}
