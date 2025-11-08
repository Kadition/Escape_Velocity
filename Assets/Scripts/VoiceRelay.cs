using System;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Netcode;


// TODO - THINGSSSS - every time a player spawns, add them to dictionary

public class VoiceRelayMono : MonoBehaviour
{
    public static VoiceRelayMono instance;
    // private MemoryStream output;
    private MemoryStream stream;

    // TODO - update this when a player spawns

    // the key is the steam id
    public Dictionary<ulong, VocalAudioPlayer> vocalAudioPlayers = new();
    // private MemoryStream input;

    // private int optimalRate;
    // private int clipBufferSize;
    // private float[] clipBuffer;

    // private int playbackBuffer;
    // private int dataPosition;
    // private int dataReceived;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // optimalRate = (int)SteamUser.OptimalSampleRate;

        // clipBufferSize = optimalRate * 5;
        // clipBuffer = new float[clipBufferSize];

        stream = new MemoryStream();
        // output = new MemoryStream();
        // input = new MemoryStream();

        // * only the host should open the socket
        if (NetworkManager.Singleton.IsHost)
        {
            VoiceRelayCreate.socketManager = SteamNetworkingSockets.CreateRelaySocket<VoiceRelayCreate>();
        }

        // source.clip = AudioClip.Create("VoiceData", (int)256, 1, (int)optimalRate, true, OnAudioRead, null);
        // source.loop = true;
        // source.Play();
        ConnectToRelay(76561198898264653);
    }

    public void ConnectToRelay(ulong hostSteamID)
    {
        // TODO - all shhould do this, even host
        VoiceRelayConnect.connectionManager = SteamNetworkingSockets.ConnectRelay<VoiceRelayConnect>(hostSteamID);
        SteamUser.VoiceRecord = true;
    }

    void Update()
    {
        if (SteamUser.HasVoiceData)
        {
            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            unsafe
            {
                fixed (byte* ptr = stream.GetBuffer())
                {
                    VoiceRelayConnect.connectionManager.Connection.SendMessage((IntPtr)ptr, compressedWritten, SendType.Unreliable);
                }
            }
        }
    }

    void OnDestroy()
    {
        SteamUser.VoiceRecord = false;
        VoiceRelayConnect.connectionManager.Connection.Close();
        VoiceRelayCreate.socketManager.Close();

        // dont do these on menu, and maybe start recording voice even in menu lol
        stream.Close();
        // input.Close();
        // output.Close();
    }

    // public void PlayAudio(byte[] compressed, int bytesWritten)
    // {
    //     input.Write(compressed, 0, bytesWritten);
    //     input.Position = 0;

    //     int uncompressedWritten = SteamUser.DecompressVoice(input, bytesWritten, output);
    //     input.Position = 0;

    //     byte[] outputBuffer = output.GetBuffer();
    //     WriteToClip(outputBuffer, uncompressedWritten);
    //     output.Position = 0;
    // }

    // void WriteToClip(byte[] uncompressed, int iSize)
    // {
    //     for (int i = 0; i < iSize; i += 2)
    //     {
    //         // insert converted float to buffer
    //         float converted = (short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f;
    //         clipBuffer[dataReceived] = converted;

    //         // buffer loop
    //         dataReceived = (dataReceived + 1) % clipBufferSize;

    //         playbackBuffer++;
    //     }
    // }

    // private void OnAudioRead(float[] data)
    // {
    //     for (int i = 0; i < data.Length; ++i)
    //     {
    //         data[i] = 0;

    //         if (playbackBuffer > 0)
    //         {
    //             // current data position playing
    //             dataPosition = (dataPosition + 1) % clipBufferSize;

    //             data[i] = clipBuffer[dataPosition];

    //             playbackBuffer --;
    //         }
    //     }
    // }

}

public class VoiceRelayConnect : ConnectionManager
{
    public static VoiceRelayConnect connectionManager;

    public ulong lastID = 0;

    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
        Debug.Log($"you have connected");
    }

    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        base.OnMessage(data, size, messageNum, recvTime, channel);

        if (size == sizeof(ulong))
        {
            lastID = (ulong)data.ToInt64();
        }
        else
        {
            if(lastID == 0)
            {
                return;
            }

            byte[] buffer = new byte[size];

            Marshal.Copy(data, buffer, 0, size);

            VoiceRelayMono.instance.vocalAudioPlayers[lastID].PlayAudio(buffer, size);
        }        
    }
}

public class VoiceRelayCreate : SocketManager
{
    public static VoiceRelayCreate socketManager;

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

            ulong steamID = identity.SteamId;

            byte[] steamIDBytes = BitConverter.GetBytes(steamID);

            unsafe
            {
                fixed (byte* ptr = steamIDBytes)
                {
                    connectionMade.SendMessage((IntPtr)ptr, sizeof(ulong), SendType.Unreliable);
                }
            }

            connectionMade.SendMessage(data, size, SendType.Unreliable);
        }
    }

    void OnDestroy()
    {
        socketManager.Close();
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