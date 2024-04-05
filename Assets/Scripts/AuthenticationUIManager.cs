using TMPro; // TMP 네임스페이스 추가
using UnityEngine;

public class AuthenticationUIManager : MonoBehaviour
{
    public TMP_InputField usernameInputField; // 사용자 이름 입력 필드
    public TMP_InputField passwordInputField; // 비밀번호 입력 필드
    public AuthenticationManager authManager; // AuthenticationManager 스크립트 참조

    // 로그인 버튼 클릭 처리
    public void OnLoginButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // 로그인 검증
        bool isValidUser = authManager.ValidateUser(username, password);
        if (isValidUser)
        {
            Debug.Log("Login successful");
            // 로그인 성공 처리
        }
        else
        {
            Debug.Log("Login failed");
            // 로그인 실패 처리
        }
    }

    // 회원 가입 버튼 클릭 처리
    public void OnRegisterButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // 회원 가입 처리
        authManager.AddUser(username, password);
        Debug.Log("Registration successful");
        // 회원 가입 성공 처리
    }
}
