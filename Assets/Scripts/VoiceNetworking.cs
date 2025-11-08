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
        VoiceRelayMono.instance.vocalAudioPlayers.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            VocalAudioPlayer vocalAudioPlayer = player.GetComponent<VocalAudioPlayer>();

            ulong steamiD = player.GetComponent<PlayerManager>().steam_id;

            VoiceRelayMono.instance.vocalAudioPlayers.Add(steamiD, vocalAudioPlayer);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void removeMeFromVocalDictionaryRpc(ulong steamID)
    {
        VoiceRelayMono.instance.vocalAudioPlayers.Remove(steamID);
    }

    [Rpc(SendTo.Server)]
    public void spawnMeInCoachRpc(ulong id)
    {
        GameObject playerInstance = Instantiate(playerPrefab);

        playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }
}
