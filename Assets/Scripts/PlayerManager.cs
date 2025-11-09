using Unity.Netcode;
using UnityEngine;
using Steamworks;

public class PlayerManager : NetworkBehaviour
{
    public ulong steam_id;

    const float velocityModifierConstant = 1;

    [SerializeField] SpringJointMaker springJointMaker;

    [SerializeField] Transform handsPosition;

    bool holdingPlayer = false;

    ulong heldPlayerId; // steam id

    // [SerializeField] Rigidbody rb;

    // [SerializeField] Rigidbody handRigidbody;
    // [SerializeField] Rigidbody connectedRigidbody;

    // ! use this for what the other person should copy
    public Transform connectedPosition;

    public Rigidbody connectedRigidbody;

    [SerializeField] GameObject playerModel;

    void Start()
    {
        Debug.Log("satrtatrar2");
        if (IsOwner)
        {
            Debug.Log("satrtatrar");
            updateIDRpc(SteamClient.SteamId);
            playerModel.SetActive(false);
        }
        else
        {
            requestIDRpc();
        }

        foreach (GameObject joint in GameObject.FindGameObjectsWithTag("Joint"))
        {
            if (joint.GetComponent<NetworkBehaviour>().OwnerClientId == OwnerClientId)
            {
                connectedPosition = joint.transform;
                connectedRigidbody = joint.GetComponent<Rigidbody>();
                return;
            }
        }

        Debug.LogError("uhoh");
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
    }


    void Update()
    {
        // if (!IsOwner)
        // {
        //     return;
        // }

        handsPosition.localPosition = new Vector3(0, 0, 1);

        if (IsOwner && Input.GetKeyUp(KeyCode.E))
        {
            if (holdingPlayer)
            {
                holdingPlayer = false;

                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

                OnReleasePlayerRpc(heldPlayerId, connectedRigidbody.linearVelocity);
            }
            else
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player == gameObject)
                    {
                        continue;
                    }

                    if (Vector3.Distance(player.transform.position, transform.position) < 2)
                    {
                        holdingPlayer = true;
                        connectedPosition.position = player.transform.position;
                        springJointMaker.MakeJoint();
                        heldPlayerId = player.GetComponent<PlayerManager>().steam_id;

                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

                        OnClickPlayerRpc(player.GetComponent<PlayerManager>().steam_id);
                        break;
                    }
                }
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void OnClickPlayerRpc(ulong id)
    {
        springJointMaker.attached = true;

        if(id == SteamClient.SteamId)
        {
            GameObject[] playerlist = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerlist)
            {
                if (player == gameObject)
                {
                    continue;
                }

                PlayerManager playerManager = player.GetComponent<PlayerManager>();

                if (playerManager.steam_id == id)
                {
                    player.GetComponent<PlayerController>().overrideMovement = true;
                    player.GetComponent<PlayerController>().placeToTransform = connectedPosition;
                    Debug.LogWarning("WEEEEEEEEEEEEEEEEEE");
                    break;
                }
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void OnReleasePlayerRpc(ulong id, Vector3 velocity)
    {
        springJointMaker.attached = false;

        if(id == SteamClient.SteamId)
        {
            GameObject[] playerlist = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerlist)
            {
                if (player.GetComponent<PlayerManager>().steam_id == SteamClient.SteamId)
                {
                    player.GetComponent<Rigidbody>().linearVelocity = velocity * velocityModifierConstant;
                    player.GetComponent<PlayerController>().overrideMovement = false;
                    Debug.LogWarning("WEEEEEEEEEEEEEEEEEE2");
                }
            }
        }
    }

    // [Rpc(SendTo.Everyone)]
    // public void OnClickPlayerRpc(ulong steamID)
    // {
    //     attachedPlayer = null;

    //     foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
    //     {
    //         if (player == gameObject)
    //         {
    //             continue;
    //         }

    //         if (player.GetComponent<PlayerManager>().steam_id == steamID)
    //         {
    //             attachedPlayer = player;
    //             break;
    //         }
    //     }

    //     springJointMaker.MakeJoint(attachedPlayer.GetComponent<Rigidbody>());

    //     springJointMaker.connectedPlayer = attachedPlayer.transform;

    //     springJointMaker.rope.SetActive(true);

    //     springJointMaker.attached = true;

    //     rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

    //     holdingPlayer = true;
    // }
    
    // [Rpc(SendTo.Everyone)]
    // public void OnReleasePlayerRpc()
    // {
    //     attachedPlayer = null;

    //     springJointMaker.connectedPlayer = null;

    //     springJointMaker.attached = false;

    //     springJointMaker.rope.SetActive(false);

    //     holdingPlayer = false;

    //     rb.constraints = RigidbodyConstraints.None;

    //     rb.constraints = RigidbodyConstraints.FreezeRotation;

    //     Destroy(GetComponent<SpringJoint>());
    // }
}
