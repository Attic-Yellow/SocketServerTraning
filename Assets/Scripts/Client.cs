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

public class Client : MonoBehaviour
{
    private StreamReader reader;
    private StreamWriter writer;

    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private RectTransform textArea;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject textBox;
    [SerializeField] private int entryCount;

    [SerializeField] private string userName;

    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private TMP_InputField port;

    [SerializeField] private Queue<string> log = new Queue<string>();

    public void ClientConnectButtonClick()
    {
        // ������ ������ ������ ����
        Thread thread = new Thread(ClientStart);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ClientStart()
    {
        try 
        {
            TcpClient tcpClient = new TcpClient(); // ���� ��� Ŭ���̾�Ʈ ��ü ����
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress.text), int.Parse(port.text)); // ���� ���� ��Ŷ�� ���޵� �������� ���� �����Ͱ� ���ǵǾ� �ִ� IPEndPointŬ���� ����
                        
            tcpClient.Connect(endPoint); //���� ������ �����Ͽ� listener�� Ȧ���� �� �ֵ��� ��

            log.Enqueue("Client Connected");
            userName = "new User";

            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            writer.AutoFlush = true;

            while(tcpClient.Connected)
            {
                string readString = reader.ReadLine();
                log.Enqueue(readString); 
            }
        }
        catch (ApplicationException e) 
        {
            log.Enqueue($"Exception Caused : {e.Message}");
        }
        catch (Exception e)
        {
            log.Enqueue($" {e.Message}");
        }
    }

    public void MassageToServer(string message)
    {
        writer.WriteLine($"  {userName} : {message}");
        log.Enqueue($"  {userName} : {message}");
        Contant();
    }

    public void Flush()
    {
        writer.Flush(); // AutoFlush�� true�� �ƴҶ� �ʿ���
    }

    private void Update()
    {
        if (log.Count > 0)
        {
            
            var logText = Instantiate(this.logText, textArea);
            logText.text += $"{log.Dequeue()}";
        }
    }

    private void Contant()
    {
        entryCount++;
        float entryHeight = 80f;
        float totalHeight = (entryCount + 1) * entryHeight;
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }
}
