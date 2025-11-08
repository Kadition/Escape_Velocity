using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using System.Collections;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager instance { get; private set; } = null;
    private FacepunchTransport transport = null;

    [SerializeField] GameObject canvas;

    public Lobby? currentLobby { get; private set; } = null;

    public ulong hostId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;

        // StartCoroutine("CheckConnection");
    }

    // private IEnumerator CheckConnection()
    // {
    //     UnityWebRequest webRequest = UnityWebRequest.Get("www.example.com");

    //     yield return webRequest.SendWebRequest();

    //     if (webRequest.result == UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("Connection successful");
    //     }
    //     else
    //     {
    //         Debug.Log("Connection unsucuessful");
    //     }
    // }

    void OnDestroy()
    {

        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;

        Disconnected();
    }

    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamId)
    {
        RoomEnter joinedLobby = await _lobby.Join();
        if (joinedLobby != RoomEnter.Success)
        {
            Debug.Log("Failed to enter a lobby");

            Disconnected();
        }
        else
        {
            currentLobby = _lobby;
            Debug.Log("Joined a lobby");
        }
    }

    private void SteamMatchmaking_OnLobbyGameCreated(Lobby _lobby, uint _ip, ushort _port, SteamId _steamId)
    {
        Debug.Log("Lobby was created");
    }

    private void SteamMatchmaking_OnLobbyInvite(Friend _steamId, Lobby _lobby)
    {
        Debug.Log($"Invite from {_steamId.Name}");
    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _friend)
    {
        Debug.Log("Lobby Member Left");
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
    {
        Debug.Log("Lobby Member Joined");
    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }

        StartClient(currentLobby.Value.Owner.Id);
    }

    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            Disconnected();

            Debug.Log("Lobby was not created");
            return;
        }

        _lobby.SetPublic();
        _lobby.SetJoinable(true);
        _lobby.SetGameServer(_lobby.Owner.Id);
        Debug.Log($"Lobby Created {_lobby.Owner.Name}");
    }

    public async void StartHost(int _maxMembers)
    {
        if (!SteamClient.IsValid)
        {
            Debug.Log("Steam client isnt running");
            return;
        }

        Debug.Log("Steam client is valid and logged on.");

        Debug.Log("Started Host");

        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();

        currentLobby = await SteamMatchmaking.CreateLobbyAsync(_maxMembers);

        SteamUser.VoiceRecord = true;

        canvas.SetActive(false);

        PlayerNetworkManager.instance.spawnMeInCoachRpc(NetworkManager.Singleton.LocalClientId, SteamClient.SteamId);
    }

    public void StartClient(SteamId _sId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

        transport.targetSteamId = _sId;

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client has started");
        }

        SteamUser.VoiceRecord = true;

        canvas.SetActive(false);

        StartCoroutine(clientWait());
    }

    IEnumerator clientWait()
    {
        if(NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            yield return null;
        }
        
        // yield return new WaitForSeconds(5);

        PlayerNetworkManager.instance.spawnMeInCoachRpc(NetworkManager.Singleton.LocalClientId, SteamClient.SteamId);
    }

    public void Disconnected()
    {
        PlayerNetworkManager.instance.removeMeFromVocalDictionaryRpc(SteamClient.SteamId);

        currentLobby?.Leave();

        currentLobby = null;

        if (NetworkManager.Singleton == null)
        {
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        }

        NetworkManager.Singleton.Shutdown(true);

        // * if you lock the cursor during the game (which you should)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Disconnected");
    }

    private void Singleton_OnClientConnectedCallback(ulong _clientId)
    {
        Debug.Log($"Client has connected: {_clientId}");
    }

    private void Singleton_OnClientDisconnectCallback(ulong _clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
    }

    private void Singleton_OnServerStarted()
    {
        Debug.Log("Host Started");
    }

    public void ShowInviteFriendsScreen()
    {
        SteamFriends.OpenGameInviteOverlay(instance.currentLobby.Value.Id);
    }

    public void ShowSteamFriends()
    {
        SteamFriends.OpenOverlay("friends");
    }
}