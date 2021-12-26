using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class config
{
    private static string filename = "secrets/config.config";
    private static TextReader reader = File.OpenText(filename);
    public static string channelName { get; set; }
    public static string username { get; set; }
    public static string oauth { get; set; }

    public static string databasePath { get; set; }

    public static void parseConfig() {
        string[] configContents = reader.ReadToEnd().Split('\n');
        foreach (string s in configContents) {
            int colonIndex = s.IndexOf(':');
            if (s.StartsWith("channel name: ")) {
                channelName = s.Substring(colonIndex + 1).Trim().ToLower();
            }
            if (s.StartsWith("username: "))
            {
                username = s.Substring(colonIndex + 1).Trim().ToLower();
            }
            if (s.StartsWith("oauth: "))
            {
                oauth = s.Substring(colonIndex + 1).Trim();
            }
            if (s.StartsWith("database path: "))
            {
                databasePath = s.Substring(colonIndex + 1).Trim();
            }
        }
    }

}
