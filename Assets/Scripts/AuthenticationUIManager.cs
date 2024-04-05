using TMPro; // TMP ���ӽ����̽� �߰�
using UnityEngine;

public class AuthenticationUIManager : MonoBehaviour
{
    public TMP_InputField usernameInputField; // ����� �̸� �Է� �ʵ�
    public TMP_InputField passwordInputField; // ��й�ȣ �Է� �ʵ�
    public AuthenticationManager authManager; // AuthenticationManager ��ũ��Ʈ ����

    // �α��� ��ư Ŭ�� ó��
    public void OnLoginButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // �α��� ����
        bool isValidUser = authManager.ValidateUser(username, password);
        if (isValidUser)
        {
            Debug.Log("Login successful");
            // �α��� ���� ó��
        }
        else
        {
            Debug.Log("Login failed");
            // �α��� ���� ó��
        }
    }

    // ȸ�� ���� ��ư Ŭ�� ó��
    public void OnRegisterButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // ȸ�� ���� ó��
        authManager.AddUser(username, password);
        Debug.Log("Registration successful");
        // ȸ�� ���� ���� ó��
    }
}
