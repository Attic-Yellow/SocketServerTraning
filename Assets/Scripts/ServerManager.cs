using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // 예외 처리 (Exception)을 참조하기 위한 namespace
using System.IO; // 입출력에 관련된 namespace
using System.Net; // 네트워킹에 관련된 namespace
using System.Net.Sockets; // TCP(소켓)통신에 관련된 namespace
using System.Threading;  // 멀티 스레딩에 관련된 namespace


public class ServerManager : MonoBehaviour
{
    // 파일 입출력, 네트워킹 송수신에 활용되는 스트림
    private StreamReader reader = null;
    private StreamWriter writer = null;

    public Thread serverThread = null;

    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private RectTransform textArea;

    [SerializeField] private string ipAddress = "127.0.0.1";
    [SerializeField] private int port = 9999;

    public static Queue<string> log = new Queue<string>();

    private List<ClientHandler> clientList = new List<ClientHandler>();

    [SerializeField] private bool isServerConnected;

    public void ServerConnectButtonClick()
    {
        if (!isServerConnected)
        {
            // 서버를 열어야 함
            serverThread = new Thread(ServerStart);
            serverThread.IsBackground = true;
            serverThread.Start();
            isServerConnected = !isServerConnected;
        }
        else
        {
            serverThread.Abort();
            isServerConnected = !isServerConnected;
        }
    }
    private int clientId = 0;
    // 서버에서 패킷을 받는 프로세스를 실행
    private void ServerStart()
    {
        try
        {
            // 지속적으로 호출 됨(Update와 흡사한 기능, multithread로 구현)
            TcpListener tcpSocjketListener = new TcpListener(IPAddress.Parse(ipAddress), port);

            tcpSocjketListener.Start();

            log.Enqueue("Server Start");

            while (true)
            {
                TcpClient client = tcpSocjketListener.AcceptTcpClient();

                ClientHandler handler = new ClientHandler();

                handler.Connect(client, this, clientId++);

                clientList.Add(handler);

                Thread clientThread = new Thread(handler.Run);
                clientThread.IsBackground = true;
                clientThread.Start();

                log.Enqueue($"{handler.id} Client Conneted");
            }

            return;
            TcpClient tcpClient = tcpSocjketListener.AcceptTcpClient();

            log.Enqueue("Client Connected");

            // 접속된 클라이언트로부터 송수신 스트림 생성
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            writer.AutoFlush = true;

            while (tcpClient.Connected)
            {
                string readString = reader.ReadLine();

                if (string.IsNullOrEmpty(readString))
                {
                    continue;
                }
                log.Enqueue(readString);
            }
        }
        catch (ApplicationException e)
        {
            log.Enqueue($"Application Exceotion Caused {e.Message}");
        }
        catch (Exception e)
        {
            log.Enqueue($"Server Excption Caused {e.Message}");
        }
    }

    private void Update()
    {
        if (log.Count > 0)
        {
            var logText = Instantiate(this.logText, textArea);
            logText.text = log.Dequeue();
        }
    }

    public void BroadcastToClients(string message)
    {
        log.Enqueue(message);

        foreach (ClientHandler client in clientList)
        {
            client.MessageToClient(message);
        }
    }
}

public class ClientHandler
{
    public int id;
    public ServerManager server;
    public TcpClient client;
    public StreamReader reader;
    public StreamWriter writer;

    public void Connect(TcpClient client, ServerManager manager, int id)
    {
        this.server = manager;
        this.id = id;
        this.client = client;
        reader = new StreamReader(client.GetStream());
        writer = new StreamWriter(client.GetStream());
        writer.AutoFlush = true;
    }

    public void DisConnect()
    {
        writer.Close();
        reader.Close();
        client.Close();
    }

    public void MessageToClient(string message)
    {
        writer.WriteLine(message);
    }

    public void Run()
    {
        try
        {
            while (client.Connected)
            {
                string readString = reader.ReadLine();
                if (string.IsNullOrEmpty(readString))
                {
                    continue;
                }

                //ServerManager.log.Enqueue(readString);
                //MessageToClient(readString);

                server.BroadcastToClients($"{id} : {readString}");

            }

        }
        catch
        {

        }
        finally
        {
            DisConnect();
        }
    }
}
