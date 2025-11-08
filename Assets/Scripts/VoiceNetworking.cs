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
        Debug.Log(id + " " + steamID);

        GameObject playerInstance = Instantiate(playerPrefab);

        playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }

    // public void spawnMeInCoachRpc()

    // TODO - later do not me
    [Rpc(SendTo.Everyone)]
    public void voiceDataRpc(byte[] data, ulong steamID, int size)
    {
        if (VoiceRelay.instance.vocalAudioPlayers.ContainsKey(steamID))
        {
            VoiceRelay.instance.vocalAudioPlayers[steamID].playAudio(data, size);
        }
    }
}
