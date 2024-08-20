using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UnityClient : MonoBehaviour
{
    public struct AIInfo
    {
        public string Prompt;
        public string aiAnswer;
    }

    public string prompt;
    public string serverIP = "127.0.0.1";
    public int serverPort = 25001;
    private string logPath;
    private AIInfo MyAIInfo = new AIInfo
    {
        Prompt = "up好可爱",
    };

    private TcpClient client;
    private NetworkStream stream;
    private string realSendPrompt;
    private string lastString;

    void Start()
    {
        logPath = Application.persistentDataPath + "/LLM_log.json";
        ConnectToServer();
        InvokeRepeating("SendTimedMessage", 10f, 12f);  // 每20秒调用一次SendTimedMessage
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

    void SendTimedMessage()
    {
        if(MyAIInfo.Prompt != lastString)
        {
            lastString = MyAIInfo.Prompt;
            realSendPrompt = MyAIInfo.Prompt;
            SendMessage(MyAIInfo);
        }
        
    }

    void SendMessage(AIInfo aiInfo)
    {
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
        System.IO.File.WriteAllText(logPath, json);
        AIInfo aiInfo = JsonUtility.FromJson<AIInfo>(json);
        MyAIInfo.Prompt = (prompt=="") ? MyAIInfo.Prompt : prompt;
        MyAIInfo.aiAnswer = aiInfo.aiAnswer;
        Debug.Log("Prompt：" + aiInfo.Prompt + "，AI Answer：" + aiInfo.aiAnswer);
    }

    // 新增类，用于外部调用
    public class AIInterface
    {
        private UnityClient clientInstance;

        public AIInterface(UnityClient instance)
        {
            clientInstance = instance;
            Debug.LogError(instance == null ? "myinstance is null" : "myinstance is not null");
        }

        // 外部调用此方法设置Prompt
        public void SetPrompt(string prompt)
        {
            Debug.LogError("Cli" + prompt);
            Debug.LogError(clientInstance == null ? "myObject is null" : "myObject is not null");
            clientInstance.prompt = prompt;
            Debug.LogError("CliL" + prompt);
        }

        // 外部调用此方法获取AI的回答
        public string GetAnswer()
        {
            return clientInstance.MyAIInfo.aiAnswer;
        }

        public string GetChosenText()
        {
            return clientInstance.realSendPrompt;
        }
    }

    void OnDestroy()
    {
        stream.Close();
        client.Close();
    }
}
