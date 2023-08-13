using System.Collections;
using System.Collections.Generic;
using Tashi.NetworkTransport;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbyUpdate : LobbyAbstract
{
    public TashiNetworkTransport NetworkTransport => NetworkManager.Singleton.NetworkConfig.NetworkTransport as TashiNetworkTransport;




}
