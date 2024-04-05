using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private string connString;

    void Start()
    {
        // �����ͺ��̽� ���� ��� ����
        connString = "URI=file:" + Application.persistentDataPath + "/UserDatabase.db";
        Debug.Log("Database Path: " + connString);

        // �����ͺ��̽� ���� �� ����� ���̺� ����
        CreateTable();
    }

    // ����� ���̺� ����
    void CreateTable()
    {
        using (var conn = new SqliteConnection(connString))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE, password TEXT);";

                var result = cmd.ExecuteNonQuery();
                Debug.Log("Create Table Result: " + result);
            }
        }
    }
}
