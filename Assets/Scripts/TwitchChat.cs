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

    private string username, password, channelName; //Get the password from https://twitchapps.com/tmi
   
    void Start() {
        config.parseConfig();
        username = config.username;
        password = config.oauth;
        channelName = config.channelName;
        Connect();
        Database.Start();
    }

    void OnEnable()
    {
        writeChat("helloWorld");
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

            string output = message;

            if (message.Contains("PING :"))
            {
                writeChat("PONG :tmi.twitch.tv");
            }

            if (message.Contains("PRIVMSG #"))
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

                if (body.StartsWith("!")) {
                    handleCommand(body, username);
                }
            }

            Debug.Log(output);
        }
    }

    private int handleCommand(string command, string username) {

        switch (command.ToLower()) {
          
        }
        return -1;
    }

}
