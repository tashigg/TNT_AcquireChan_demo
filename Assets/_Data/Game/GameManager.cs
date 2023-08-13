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

        this.ReceiveIncomingDetail();
    }

    protected virtual void ReceiveIncomingDetail()
    {
        Debug.LogWarning("Receive Incoming Detail");
        var incomingSessionDetails = IncomingSessionDetails.FromUnityLobby(LobbyManager.Instance.lobby);
        NetworkTransport.UpdateSessionDetails(incomingSessionDetails);
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
        }
    }

    protected virtual async void LobbyUpdating()
    {
        var outgoingSessionDetails = NetworkTransport.OutgoingSessionDetails;

        var updatePlayerOptions = new UpdatePlayerOptions();
        if (outgoingSessionDetails.AddTo(updatePlayerOptions))
        {
            await LobbyService.Instance.UpdatePlayerAsync(LobbyManager.Instance.lobbyId, LobbyManager.Instance.playerId, updatePlayerOptions);
        }

        if (LobbyManager.Instance.isHost)
        {
            var updateLobbyOptions = new UpdateLobbyOptions();
            if (outgoingSessionDetails.AddTo(updateLobbyOptions))
            {
                await LobbyService.Instance.UpdateLobbyAsync(LobbyManager.Instance.lobbyId, updateLobbyOptions);
            }
        }
    }
}
