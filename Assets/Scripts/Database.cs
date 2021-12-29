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
    }

    public static void Stop() {
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public static int registerUser(int userID, string username) {
        string sqlQuery = "INSERT INTO 'Users' ('UserID', 'Username') VALUES ('" + userID + "', '" + username + "')";
        dbcmd.CommandText = sqlQuery;
        return dbcmd.ExecuteNonQuery();
    }

    public static bool checkIsRegistered(int userID, string username) {
        string sqlQuery = "SELECT COUNT (*) FROM 'Users' WHERE UserID = '" + userID + "' AND Username = '" + username + "'";
        dbcmd.CommandText = sqlQuery;
        return int.Parse(dbcmd.ExecuteScalar().ToString()) > 0;
    }

    public static bool checkIsRegistered(string username) {
        string sqlQuery = "SELECT COUNT (*) FROM 'Users' WHERE Username = '" + username + "'";
        dbcmd.CommandText = sqlQuery;
        return int.Parse(dbcmd.ExecuteScalar().ToString()) > 0;
    }

}