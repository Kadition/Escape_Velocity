using Unity.Netcode;
using UnityEngine;
using Steamworks;
using Steamworks.Data;


public class VoiceNetworking : NetworkBehaviour
{
    public static VoiceNetworking instance;
    [SerializeField] private GameObject playerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

    [Rpc(SendTo.Everyone)]
    public void updateVocalDictionaryRpc()
    {
        VoiceRelay.instance.vocalAudioPlayers.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            VocalAudioPlayer vocalAudioPlayer = player.GetComponent<VocalAudioPlayer>();

            ulong steamiD = player.GetComponent<PlayerManager>().steam_id;

            Debug.Log("steam id: " + steamiD);

            VoiceRelay.instance.vocalAudioPlayers.Add(steamiD, vocalAudioPlayer);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void removeMeFromVocalDictionaryRpc(ulong steamID)
    {
        VoiceRelay.instance.vocalAudioPlayers.Remove(steamID);
    }

    [Rpc(SendTo.Server)]
    public void spawnMeInCoachRpc(ulong id, ulong steamID)
    {
        GameObject playerInstance = Instantiate(playerPrefab);

        playerInstance.GetComponent<PlayerManager>().steam_id = steamID;

        playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }

    // TODO - later do not me
    [Rpc(SendTo.Everyone)]
    public void voiceDataRpc(byte[] data, ulong steamID, int size)
    {
        VoiceRelay.instance.vocalAudioPlayers[steamID].playAudio(data, size);
    }
}
