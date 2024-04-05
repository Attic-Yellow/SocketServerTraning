using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    private string connString;

    void Awake()
    {
        // 데이터베이스 파일 경로 설정
        connString = "URI=file:" + Application.persistentDataPath + "/UserDatabase.db";
        Debug.Log("Database Path: " + connString);

        // 데이터베이스 연결 및 사용자 테이블 생성
        CreateTable();
    }

    // 사용자 테이블 생성
    private void CreateTable()
    {
        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE, password TEXT);";

                cmd.ExecuteNonQuery();
                Debug.Log("User table created successfully.");
            }
        }
    }

    // 사용자 추가
    public void AddUser(string username, string password)
    {
        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password);";
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                cmd.ExecuteNonQuery();
                Debug.Log("User added successfully.");
            }
        }
    }

    // 사용자 검증
    public bool ValidateUser(string username, string password)
    {
        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM users WHERE username=@username AND password=@password;";
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Debug.Log("User validated successfully.");
                        return true;
                    }
                    else
                    {
                        Debug.Log("User validation failed.");
                        return false;
                    }
                }
            }
        }
    }
}
