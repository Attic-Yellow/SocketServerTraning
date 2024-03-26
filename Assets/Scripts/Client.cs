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

    [SerializeField] private GameObject logTextPrefab; // UI Text Prefab
    [SerializeField] private RectTransform content; // 메시지들을 담을 부모 컨테이너
    [SerializeField] private GameObject textBoxPrefab; // 개별 메시지를 담을 텍스트 박스 프리팹

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
        catch (Exception e)
        {
            log.Enqueue($"Exception Caused : {e.Message}");
        }
    }

    public void MassageToServer(string message)
    {
        writer.WriteLine($"{userName} : {message}");
        log.Enqueue($"{userName} : {message}");
        UpdateContentHeight();
    }

    public void Flush()
    {
        writer.Flush(); // AutoFlush가 true가 아닐때 필요함
    }

    private void Update()
    {
        if (log.Count > 0)
        {
            string message = log.Dequeue();
            var logTextBox = Instantiate(textBoxPrefab, content); // TextBox 인스턴스 생성
            var logText = logTextBox.GetComponentInChildren<TextMeshProUGUI>(); // TextBox 내의 TextMeshProUGUI 컴포넌트 찾기
            logText.text = message; // 메시지 설정
            logText.rectTransform.sizeDelta = new Vector2(920, logText.rectTransform.sizeDelta.y);
            logText.enableWordWrapping = true; // 단어 줄바꿈 활성화
            logText.overflowMode = TextOverflowModes.Overflow; // 텍스트가 박스를 넘어갈 경우 처리 방식 설정
            AdjustTextBoxHeight(logText, logTextBox); // 이전에 제공된 메서드를 호출하여 TextBox의 높이 조절
        }
    }

    private void AdjustTextBoxHeight(TextMeshProUGUI logText, GameObject logTextBox)
    {
        RectTransform layoutElement = logTextBox.GetComponent<RectTransform>();
        if (layoutElement == null)
        {
            layoutElement = logTextBox.AddComponent<RectTransform>();
        }

        // TextMeshProUGUI의 preferredHeight를 사용하여 높이를 설정
        float textHeight = logText.preferredHeight;
        float textWidth = logText.preferredWidth;
        layoutElement.sizeDelta = new Vector2(layoutElement.sizeDelta.x, textHeight + 20);// 여분의 여백을 추가 (예: 상하 10씩)
    }

    private void UpdateContentHeight()
    {
        // 모든 자식 객체들의 높이 합계를 구하여 콘텐트의 높이를 조절
        float totalHeight = 0f;
        foreach (RectTransform child in content)
        {
            LayoutElement le = child.GetComponent<LayoutElement>();
            if (le != null)
            {
                totalHeight += le.minHeight;
            }
            else
            {
                // LayoutElement가 없는 경우 기본 높이 사용
                totalHeight += 80f; // 기본값
            }
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }
}
