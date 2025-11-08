using System;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;


// TODO - THINGSSSS - every time a player spawns, add them to dictionary

public class VoiceRelay : MonoBehaviour
{
    public static VoiceRelay instance;
    private MemoryStream stream;

    // TODO - update this when a player spawns

    // the key is the steam id
    public Dictionary<ulong, VocalAudioPlayer> vocalAudioPlayers = new();

    bool good = false;

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
        stream = new MemoryStream();
    }

    void Update()
    {

        if(good || (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening))
        {
            good = true;
        }

        if (SteamUser.HasVoiceData && good)
        {
            Debug.Log("has voice data");
            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            VoiceNetworking.instance.voiceDataRpc(stream.GetBuffer(), SteamClient.SteamId, compressedWritten);
        }
    }

    void OnDestroy()
    {
        SteamUser.VoiceRecord = false;

        // dont do these on menu, and maybe start recording voice even in menu lol
        stream.Close();
    }
}