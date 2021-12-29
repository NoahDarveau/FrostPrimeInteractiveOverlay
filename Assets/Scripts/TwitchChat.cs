using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class TwitchChat : MonoBehaviour {

    private TcpClient TwitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    public bool tags;

    private string username, password, channelName; //Get the password from https://twitchapps.com/tmi
   
    void Start() {
        config.parseConfig();
        username = config.username;
        password = config.oauth;
        channelName = config.channelName;
        Connect();
        Database.Start();
    }

    
    void Update() {
        if (!TwitchClient.Connected) {
            Connect();
            Debug.Log("Reconnecting");
        }
        _ = readChat();
    }

    async Task readChat() {
        string message = await reader.ReadLineAsync();
        onMessage(message);
    }

    void OnApplicationQuit()
    {
        Database.Stop();
        writeChat("PART #", channelName);
        readChat();
        TwitchClient.Close();
        
    }

    public void onButtonPressed(string str) {
        writeChat(str);
    }

    private void Connect() {
        TwitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(TwitchClient.GetStream());
        writer = new StreamWriter(TwitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        if (tags) {
            writer.WriteLine("CAP REQ :twitch.tv/tags");
        }
        writer.Flush();
        //Debug.Log(reader.ReadLine());
        Debug.Log("Connected Status: " + TwitchClient.Connected);
    }

    async Task writeChat(params string[] list) {
        string message = "";
        foreach (string s in list) {
            message += s;
        }
        
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
        Debug.Log("Writing to Chat: " + message);
    }

    private void onMessage(string message) {
        if (message != null) {

            Debug.Log("Raw Message: " + message);

            string output = message;

            if (message.Contains("PING :"))
            {
                writeChat("PONG :tmi.twitch.tv");
            }

            if (message.Contains("PRIVMSG #"))
            {

                if (!tags)
                {
                    //string substringKey = "#" + channelName + " :";
                    //output = message.Substring(message.IndexOf(substringKey) + substringKey.Length);
                    int exclamationPointPosition = message.IndexOf("!");
                    string username = message.Substring(1, exclamationPointPosition - 1);
                    //Skip the first character, the first colon, then find the next colon
                    int secondColonPosition = message.IndexOf(':', 1);//the 1 here is what skips the first character
                    string body = message.Substring(secondColonPosition + 1);//Everything past the second colon
                    string channel = message.TrimStart('#');
                    output = username + ": " + body;

                    Debug.Log("Username: " + username + ", body: " + body);

                    if (body.StartsWith("!")) {
                        handleCommand(body, username);
                    }
                }
                else if (tags) {
                    int userIDindex = message.IndexOf("user-id=") + 8;
                    int userID = int.Parse(message.Substring(userIDindex, 8));
                    int usernameStartIndex = message.IndexOf("display-name=") + 13;
                    int usernameEndIndex = message.IndexOf(";emotes");
                    string username = message.Substring(usernameStartIndex, usernameEndIndex - usernameStartIndex);
                    string smallerString = message.Substring(message.IndexOf("PRIVMSG") + 1, message.Length - (message.IndexOf("PRIVMSG") + 1));
                    string body = smallerString.Substring(smallerString.IndexOf(":") + 1, smallerString.Length - (smallerString.IndexOf(":") + 1));
                    Debug.Log("UserID: " + userID + ", Username: " + username + ", Body: " + body);

                    if (body.StartsWith("!")) {
                        handleCommand(body, username, userID);
                    }

                }
            }
        }
    }

    private int handleCommand(string command, string username) {

        switch (command.ToLower()) {
            case "!amiregistered":           
                if (Database.checkIsRegistered(username))
                {
                    writeChat("PRIVMSG #" + channelName + " :@" + username + ", you are registered");
                }
                else
                {
                    writeChat("PRIVMSG #" + channelName + " :@" + username + ", you are NOT registered");
                }
                break;

        }
        return -1;
    }

    private int handleCommand(string command, string username, int userID)
    {

        switch (command.ToLower())
        {
            case "!registerme":
                if (Database.registerUser(userID, username) > 0) {
                    writeChat("PRIVMSG #" + channelName + " :@" + username + " has been succesfully registered");
                } else {
                    writeChat("PRIVMSG #" + channelName + " :Something went wrong trying to register @" + username + "." +
                        " This may be because you are already registerd");
                }
                break;

            case "!amiregistered":
                Debug.Log(Database.checkIsRegistered(userID, username));
                if (Database.checkIsRegistered(userID, username)) {
                    writeChat("PRIVMSG #" + channelName + " :@" + username + ", you are registered");
                } else {
                    writeChat("PRIVMSG #" + channelName + " :@" + username + ", you are NOT registered");
                }
                break;

        }
        return -1;
    }

}
