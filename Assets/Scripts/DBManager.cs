using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private string connString;

    void Start()
    {
        // 데이터베이스 파일 경로 설정
        connString = "URI=file:" + Application.persistentDataPath + "/UserDatabase.db";
        Debug.Log("Database Path: " + connString);

        // 데이터베이스 연결 및 사용자 테이블 생성
        CreateTable();
    }

    // 사용자 테이블 생성
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
