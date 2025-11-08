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

    bool oneSec = false;

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

        StartCoroutine(oneSecEnumor());
    }

    IEnumerator oneSecEnumor()
    {
        yield return new WaitForSeconds(1);

        oneSec = true;
    }

    void Update()
    {
        if (SteamUser.HasVoiceData && oneSec)
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