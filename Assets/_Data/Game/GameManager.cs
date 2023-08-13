using System.Collections;
using System.Collections.Generic;
using Tashi.NetworkTransport;
using Unity.Netcode;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public NetworkManager networkManager;
    public TashiNetworkTransport NetworkTransport => NetworkManager.Singleton.NetworkConfig.NetworkTransport as TashiNetworkTransport;

    protected override void Start()
    {
        base.Start();
        this.GameStart();
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
}
