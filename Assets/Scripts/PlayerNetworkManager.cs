using Unity.Netcode;
using UnityEngine;


public class PlayerNetworkManager : NetworkBehaviour
{
    public static PlayerNetworkManager instance;
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject connectPrefab;
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

        GameObject connectInstance = Instantiate(connectPrefab);

        connectInstance.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }

    [Rpc(SendTo.NotMe)]
    public void voiceDataRpc(byte[] data, ulong steamID, int size)
    {
        if (VoiceRelay.instance.vocalAudioPlayers.ContainsKey(steamID))
        {
            Debug.Log("player exist in voice data");
            VoiceRelay.instance.vocalAudioPlayers[steamID].playAudio(data, size);
        }
        else
        {
            Debug.Log("player doesnt exist in voice data");
        }
    }
}
