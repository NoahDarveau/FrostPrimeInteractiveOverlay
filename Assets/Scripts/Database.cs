using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine;

public static class Database {

    private static IDbCommand dbcmd;
    private static IDbConnection dbconn;

    public static void Start() {
        string conn = "Data Source=" + Application.dataPath + config.databasePath;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT UserID " + "FROM Users";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read()) {
            int userID = reader.GetInt32(0);

            Debug.Log("UserID = " + userID);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
 
}