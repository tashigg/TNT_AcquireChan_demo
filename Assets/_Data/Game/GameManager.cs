using Tashi.NetworkTransport;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public NetworkManager networkManager;
    public TashiNetworkTransport NetworkTransport => NetworkManager.Singleton.NetworkConfig.NetworkTransport as TashiNetworkTransport;
    public float _nextHeartbeat;
    public float _nextLobbyRefresh;

    protected override void Start()
    {
        base.Start();
        this.GameStart();
    }

    void Update()
    {
        this.BattleUpdate();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadNetworkManager();
    }

    protected virtual void LoadNetworkManager()
    {
        if (this.networkManager != null) return;
        this.networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        Debug.LogWarning(transform.name + ": LoadNetworkManager", gameObject);
    }

    protected virtual void GameStart()
    {
        if (LobbyManager.Instance.isHost) this.networkManager.StartHost();
        else this.networkManager.StartClient();
    }

    protected virtual async void ReceiveIncomingDetail()
    {
        if (NetworkTransport.SessionHasStarted) return;
        Debug.LogWarning("Receive Incoming Detail");

        string lobbyId = LobbyManager.Instance.lobbyId;
        var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
        var incomingSessionDetails = IncomingSessionDetails.FromUnityLobby(lobby);

        if (incomingSessionDetails.AddressBook.Count == lobby.Players.Count)
        {
            Debug.LogWarning("Update Session Details");
            NetworkTransport.UpdateSessionDetails(incomingSessionDetails);
        }
    }

    protected async void BattleUpdate()
    {
        if (!LobbyManager.Instance.IsReady()) return;

        if (Time.realtimeSinceStartup >= this._nextHeartbeat && LobbyManager.Instance.isHost)
        {
            _nextHeartbeat = Time.realtimeSinceStartup + 15;
            await LobbyService.Instance.SendHeartbeatPingAsync(LobbyManager.Instance.lobbyId);
        }

        if (Time.realtimeSinceStartup >= _nextLobbyRefresh)
        {
            this._nextLobbyRefresh = Time.realtimeSinceStartup + 2;
            this.LobbyUpdating();
            this.ReceiveIncomingDetail();
        }
    }

    protected virtual async void LobbyUpdating()
    {
        var outgoingSessionDetails = NetworkTransport.OutgoingSessionDetails;

        var updatePlayerOptions = new UpdatePlayerOptions();
        string lobbyId = LobbyManager.Instance.lobbyId;
        string playerId = LobbyManager.Instance.playerId;

        if (outgoingSessionDetails.AddTo(updatePlayerOptions))
        {
            await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, updatePlayerOptions);
        }

        if (LobbyManager.Instance.isHost)
        {
            var updateLobbyOptions = new UpdateLobbyOptions();
            if (outgoingSessionDetails.AddTo(updateLobbyOptions))
            {
                await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateLobbyOptions);
            }
        }
    }
}
