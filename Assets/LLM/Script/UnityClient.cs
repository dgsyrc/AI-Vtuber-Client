using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityClient : MonoBehaviour
{
    public struct AIInfo
    {
        public string Prompt;
        public string aiAnswer;
    }

    public string prom;
    public string serverIP = "127.0.0.1";
    public int serverPort = 25001;
    private bool Idle = true;
    private AIInfo MyAIInfo = new AIInfo
    {
        Prompt = "up好可爱",
    };

    private TcpClient client;
    private NetworkStream stream;
    private int tsID=0;
    private int lastTsID=0;

    void Start()
    {
        ConnectToServer();
        //InvokeRepeating("SendTimedMessage", 10f, 20f);  // 每20秒调用一次SendTimedMessage
        StartCoroutine(SendTimedMessage());
    }

    void ConnectToServer()
    {
        client = new TcpClient(serverIP, serverPort);
        stream = client.GetStream();
        Debug.Log("成功连接到服务器");
    }

    void Update()
    {
        ReceiveMessage();
    }

    private IEnumerator SendTimedMessage()
    {
        while (true)
        {
            if (Idle && tsID != lastTsID)
            {
                lastTsID = tsID;
                SendMessage(MyAIInfo);
            }
            yield return new WaitForSeconds(4f);
        }
       
    }

    void SendMessage(AIInfo aiInfo)
    {
        Idle = false;
        string json = JsonUtility.ToJson(aiInfo);
        byte[] data = Encoding.UTF8.GetBytes(json);
        stream.Write(data, 0, data.Length);
        
    }

    void ReceiveMessage()
    {
        if (stream.DataAvailable)
        {
            byte[] responseData = new byte[1024];
            int bytesRead = stream.Read(responseData, 0, responseData.Length);
            string response = Encoding.UTF8.GetString(responseData, 0, bytesRead);
            DecodeJSON(response);
        }
    }

    public void DecodeJSON(string json)
    {
        AIInfo aiInfo = JsonUtility.FromJson<AIInfo>(json);
        MyAIInfo.Prompt = (prom == "") ? MyAIInfo.Prompt : prom;
        MyAIInfo.aiAnswer = aiInfo.aiAnswer;
        Debug.Log("Prompt：" + aiInfo.Prompt + "，AI Answer：" + aiInfo.aiAnswer);
        //Idle = true;
    }

    // 新增类，用于外部调用
    public class AIInterface
    {
        private UnityClient clientInstance;

        public AIInterface(UnityClient instance)
        {
            clientInstance = instance;
        }

        // 外部调用此方法设置Prompt
        public void SetPrompt(string prompt)
        {
            clientInstance.MyAIInfo.Prompt = prompt;
            clientInstance.prom = prompt;
        }

        // 外部调用此方法获取AI的回答
        public string GetAnswer()
        {
            return clientInstance.MyAIInfo.aiAnswer;
        }

        public void SetIdle()
        {
            clientInstance.Idle = true;
        }

        public string GetChosenText()
        {
            return clientInstance.MyAIInfo.Prompt;
        }
        public void SetTsID(int tsID)
        {
            clientInstance.tsID = tsID;
        }
    }

    void OnDestroy()
    {
        stream.Close();
        client.Close();
    }
}
