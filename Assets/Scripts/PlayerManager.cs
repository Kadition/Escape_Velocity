using Unity.Netcode;
using UnityEngine;
using Steamworks;
using System.Collections;

public class PlayerManager : NetworkBehaviour
{
    public ulong steam_id;

    void Start()
    {
        Debug.Log("satrtatrar2");
        if (IsOwner)
        {
            Debug.Log("satrtatrar");
            // updateIDRpc(SteamClient.SteamId);
        }
        else
        {
            // requestIDRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    public void requestIDRpc()
    {
        if(IsOwner)
        {
            updateIDRpc(SteamClient.SteamId);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void updateIDRpc(ulong newSteamID)
    {
        steam_id = newSteamID;
        Debug.Log("happened " + newSteamID);

        VoiceRelay.instance.vocalAudioPlayers.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            VocalAudioPlayer vocalAudioPlayer = player.GetComponent<VocalAudioPlayer>();

            ulong steamiD = player.GetComponent<PlayerManager>().steam_id;

            Debug.Log("steam id: " + steamiD);

            VoiceRelay.instance.vocalAudioPlayers.Add(steamiD, vocalAudioPlayer);
        }

        StartCoroutine(enumeratorThingy());
    }

    IEnumerator enumeratorThingy()
    {
        yield return new WaitForSeconds(5);

        VoiceRelay.instance.vocalAudioPlayers.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            VocalAudioPlayer vocalAudioPlayer = player.GetComponent<VocalAudioPlayer>();

            ulong steamiD = player.GetComponent<PlayerManager>().steam_id;

            Debug.Log("steam id: " + steamiD);

            VoiceRelay.instance.vocalAudioPlayers.Add(steamiD, vocalAudioPlayer);
        }
    }
}
