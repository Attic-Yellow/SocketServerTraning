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

    [SerializeField] private GameObject logTextPrefab; // UI Text Prefab
    [SerializeField] private RectTransform content; // �޽������� ���� �θ� �����̳�
    [SerializeField] private GameObject textBoxPrefab; // ���� �޽����� ���� �ؽ�Ʈ �ڽ� ������

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
        writer.Flush(); // AutoFlush�� true�� �ƴҶ� �ʿ���
    }

    private void Update()
    {
        if (log.Count > 0)
        {
            string message = log.Dequeue();
            var logTextBox = Instantiate(textBoxPrefab, content); // TextBox �ν��Ͻ� ����
            var logText = logTextBox.GetComponentInChildren<TextMeshProUGUI>(); // TextBox ���� TextMeshProUGUI ������Ʈ ã��
            logText.text = message; // �޽��� ����
            logText.rectTransform.sizeDelta = new Vector2(920, logText.rectTransform.sizeDelta.y);
            logText.enableWordWrapping = true; // �ܾ� �ٹٲ� Ȱ��ȭ
            logText.overflowMode = TextOverflowModes.Overflow; // �ؽ�Ʈ�� �ڽ��� �Ѿ ��� ó�� ��� ����
            AdjustTextBoxHeight(logText, logTextBox); // ������ ������ �޼��带 ȣ���Ͽ� TextBox�� ���� ����
        }
    }

    private void AdjustTextBoxHeight(TextMeshProUGUI logText, GameObject logTextBox)
    {
        RectTransform layoutElement = logTextBox.GetComponent<RectTransform>();
        if (layoutElement == null)
        {
            layoutElement = logTextBox.AddComponent<RectTransform>();
        }

        // TextMeshProUGUI�� preferredHeight�� ����Ͽ� ���̸� ����
        float textHeight = logText.preferredHeight;
        float textWidth = logText.preferredWidth;
        layoutElement.sizeDelta = new Vector2(layoutElement.sizeDelta.x, textHeight + 20);// ������ ������ �߰� (��: ���� 10��)
    }

    private void UpdateContentHeight()
    {
        // ��� �ڽ� ��ü���� ���� �հ踦 ���Ͽ� ����Ʈ�� ���̸� ����
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
                // LayoutElement�� ���� ��� �⺻ ���� ���
                totalHeight += 80f; // �⺻��
            }
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }
}
