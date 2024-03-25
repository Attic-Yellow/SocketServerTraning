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

    [SerializeField] private TextMeshProUGUI LogText;

    [SerializeField] private string ipAddress = "127.0.0.1";
    [SerializeField] private int port = 9999;

    [SerializeField] private Queue<string> log = new Queue<string>();
    [SerializeField] private Button connectButton;

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
    
    // �������� ��Ŷ�� �޴� ���μ����� ����
    private void ServerStart()
    {
        try
        {
            // ���������� ȣ�� ��(Update�� ����� ���, multithread�� ����)
            TcpListener tcpSocjketListener = new TcpListener(IPAddress.Parse(ipAddress), port);

            tcpSocjketListener.Start();

            log.Enqueue("Server Start");

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
            LogText.text += $"\n{ log.Dequeue()}";
        }
    }
}
