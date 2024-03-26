using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // ���� ó�� (Exception)�� �����ϱ� ���� namespace
using System.IO; // ����¿� ���õ� namespace
using System.Net; // ��Ʈ��ŷ�� ���õ� namespace
using System.Net.Sockets; // TCP(����)��ſ� ���õ� namespace
using System.Threading;  // ��Ƽ �������� ���õ� namespace


public class ServerManager : MonoBehaviour
{
    // ���� �����, ��Ʈ��ŷ �ۼ��ſ� Ȱ��Ǵ� ��Ʈ��
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
            // ������ ����� ��
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
    // �������� ��Ŷ�� �޴� ���μ����� ����
    private void ServerStart()
    {
        try
        {
            // ���������� ȣ�� ��(Update�� ����� ���, multithread�� ����)
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

            // ���ӵ� Ŭ���̾�Ʈ�κ��� �ۼ��� ��Ʈ�� ����
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
