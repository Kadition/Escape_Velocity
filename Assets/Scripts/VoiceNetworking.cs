using Unity.Netcode;
using UnityEngine;
using Steamworks;
using Steamworks.Data;


public class VoiceNetworking : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    [Rpc(SendTo.Server)]
    public void askForDictionaryAndSteamID(ulong steamId)
    {
        VoiceRelayMono.instance.vocalAudioPlayers.Add(steamId, );
    }

    [Rpc(SendTo.Everyone)]
    public void updateVocalDictionaryRpc()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            VocalAudioPlayer vocalAudioPlayer = player.GetComponent<VocalAudioPlayer>();

            VoiceRelayMono.instance.vocalAudioPlayers.Add(, );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
