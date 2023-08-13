using System.Collections;
using System.Collections.Generic;
using Tashi.NetworkTransport;
using Unity.Netcode;
using UnityEngine;

public class LobbyUpdate : LobbyAbstract
{
    public TashiNetworkTransport NetworkTransport => NetworkManager.Singleton.NetworkConfig.NetworkTransport as TashiNetworkTransport;

    protected override void Start()
    {
        base.Start();
        //this.LobbyStart();
    }

    protected virtual async void ReceiveIncomingDetail()
    {
        if (NetworkTransport.SessionHasStarted) return;

        Debug.LogWarning("Receive Incoming Detail");

        var incomingSessionDetails = IncomingSessionDetails.FromUnityLobby(this.lobbyManager.lobby);
        NetworkTransport.UpdateSessionDetails(incomingSessionDetails);
    }
}
