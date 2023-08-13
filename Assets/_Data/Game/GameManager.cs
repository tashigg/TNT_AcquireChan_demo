using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public NetworkManager networkManager;

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
    }
}
