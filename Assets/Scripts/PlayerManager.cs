using Unity.Netcode;
using UnityEngine;
using Steamworks;

public class PlayerManager : NetworkBehaviour
{
    public ulong steam_id;

    [SerializeField] SpringJointMaker springJointMaker;

    bool holdingPlayer = false;

    GameObject attachedPlayer = null;

    [SerializeField] Rigidbody rb;

    void Start()
    {
        Debug.Log("satrtatrar2");
        if (IsOwner)
        {
            Debug.Log("satrtatrar");
            updateIDRpc(SteamClient.SteamId);
        }
        else
        {
            requestIDRpc();
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

        //     StartCoroutine(enumeratorThingy());
    }

    // IEnumerator enumeratorThingy()
    // {
    //     yield return new WaitForSeconds(5);

    //     VoiceRelay.instance.vocalAudioPlayers.Clear();

    //     foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
    //     {
    //         VocalAudioPlayer vocalAudioPlayer = player.GetComponent<VocalAudioPlayer>();

    //         ulong steamiD = player.GetComponent<PlayerManager>().steam_id;

    //         Debug.Log("steam id: " + steamiD);

    //         VoiceRelay.instance.vocalAudioPlayers.Add(steamiD, vocalAudioPlayer);
    //     }
    // }

    void Update()
    {
        if(IsOwner && Input.GetKeyUp(KeyCode.E))
        {
            if (holdingPlayer)
            {
                OnReleasePlayerRpc();
            }
            else
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player == gameObject)
                    {
                        continue;
                    }

                    if (Vector3.Distance(player.transform.position, transform.position) < 3)
                    {
                        OnClickPlayerRpc(player.GetComponent<PlayerManager>().steam_id);
                        break;
                    }
                }
            }   
        }
    }

    [Rpc(SendTo.Everyone)]
    public void OnClickPlayerRpc(ulong steamID)
    {
        attachedPlayer = null;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player == gameObject)
            {
                continue;
            }

            if (player.GetComponent<PlayerManager>().steam_id == steamID)
            {
                attachedPlayer = player;
                break;
            }
        }

        springJointMaker.MakeJoint(attachedPlayer.GetComponent<Rigidbody>());

        springJointMaker.connectedPlayer = attachedPlayer.transform;

        springJointMaker.rope.SetActive(true);

        springJointMaker.attached = true;

        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        holdingPlayer = true;
    }
    
    [Rpc(SendTo.Everyone)]
    public void OnReleasePlayerRpc()
    {
        attachedPlayer = null;

        springJointMaker.connectedPlayer = null;

        springJointMaker.attached = false;

        springJointMaker.rope.SetActive(false);

        holdingPlayer = false;

        rb.constraints = RigidbodyConstraints.None;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Destroy(GetComponent<SpringJoint>());
    }
}
