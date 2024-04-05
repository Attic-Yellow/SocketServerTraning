using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    private string connString;

    void Awake()
    {
        // �����ͺ��̽� ���� ��� ����
        connString = "URI=file:" + Application.persistentDataPath + "/UserDatabase.db";
        Debug.Log("Database Path: " + connString);

        // �����ͺ��̽� ���� �� ����� ���̺� ����
        CreateTable();
    }

    // ����� ���̺� ����
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

    // ����� �߰�
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

    // ����� ����
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
