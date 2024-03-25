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
        // 서버와 연결할 스레드 생성
        Thread thread = new Thread(ClientStart);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ClientStart()
    {
        try 
        {
            TcpClient tcpClient = new TcpClient(); // 소켓 통신 클라이언트 객체 생성
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress.text), int.Parse(port.text)); // 내가 보낸 패킷이 전달될 목적지에 대한 데이터가 정의되어 있는 IPEndPoint클래스 생성
                        
            tcpClient.Connect(endPoint); //실제 서버를 연결하여 listener가 홀딩할 수 있도록 함

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
        writer.Flush(); // AutoFlush가 true가 아닐때 필요함
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
