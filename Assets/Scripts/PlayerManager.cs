using Unity.Netcode;
using UnityEngine;
using Steamworks;

public class PlayerManager : NetworkBehaviour
{
    public ulong steam_id;

    void Start()
    {
        if (IsOwner)
        {
            updateIDRpc(SteamClient.SteamId);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void updateIDRpc(ulong newSteamID)
    {
        steam_id = newSteamID;
    }
}
