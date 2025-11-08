using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using System.IO;

public class VoiceRelayMono : MonoBehaviour
{
    ConnectionManager connectionManager = new();
    VoiceRelayCreate voiceRelay = new();
    private MemoryStream output;
    private MemoryStream stream;
    private MemoryStream input;
    void Start()
    {
        stream = new MemoryStream();
        output = new MemoryStream();
        input = new MemoryStream();

        voiceRelay.Start();
    }

    public void ConnectToRelay(ulong hostSteamID)
    {
        // TODO - all shhould do this, even host
        if (true)
        {
            connectionManager = SteamNetworkingSockets.ConnectRelay<ConnectionManager>(hostSteamID);
        }
        SteamUser.VoiceRecord = true;
    }

    void Update()
    {
        // TODO - in lobby
        if (SteamUser.HasVoiceData)
        {
            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

        }
    }

    // TODO - or on returning to lobby
    void OnDestroy()
    {
        SteamUser.VoiceRecord = false;
    }

    public void sendIt(byte[] compressed, int bytesWritten)
    {
        unsafe
        {
            fixed (byte* ptr = compressed)
            {
                connectionManager.Connection.SendMessage((IntPtr)ptr, bytesWritten, SendType.Unreliable);
            }
        }
    }
}

public class VoiceRelayCreate : SocketManager
{
    public SocketManager socketManager;
    public Socket socket;
    public uint remoteSteamId;

    private Queue<byte[]> voiceQueue = new Queue<byte[]>();



    public void Start()
    {
        // TODO - only the host should open the socket
        if (true)
        {
            socketManager = SteamNetworkingSockets.CreateRelaySocket<SocketManager>();
        }
    }

    public override void OnConnecting(Connection connection, ConnectionInfo data)
    {
        base.OnConnecting(connection, data);
        connection.Accept();
        Debug.Log($"{data.Identity} is connecting");
    }

    public override void OnConnected(Connection connection, ConnectionInfo data)
    {
        base.OnConnected(connection, data);
        Debug.Log($"{data.Identity} has joined the relay");
    }

    public override void OnDisconnected(Connection connection, ConnectionInfo data)
    {
        base.OnDisconnected(connection, data);
        Debug.Log($"{data.Identity} is out of here");
    }

    public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        base.OnMessage(connection, identity, data, size, messageNum, recvTime, channel);
        Debug.Log($"We got a message from {identity}!");

        foreach (Connection connectionMade in socketManager.Connected)
        {
            if (connectionMade == connection)
            {
                continue;
            }

            connectionMade.SendMessage(data, size, SendType.Unreliable);
        }
    }
}


    // public void sendIt(byte[] compressed, int bytesWritten)
    // {
    //     foreach (Connection connection in socketManager.Connected)
    //     {
    //         unsafe
    //         {
    //             fixed (byte* ptr = compressed)
    //             {
    //                 connection.SendMessage((IntPtr)ptr, bytesWritten, SendType.Unreliable);
    //             }
    //         }
    //     }
    // }