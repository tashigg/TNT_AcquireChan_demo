using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public abstract class LobbyAbstract : SaiMonoBehaviour
{
    public LobbyManager lobbyManager;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadLobbyManager();
    }

    protected virtual void LoadLobbyManager()
    {
        if (this.lobbyManager != null) return;
        this.lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        Debug.LogWarning(transform.name + ": LoadLobbyManager", gameObject);
    }
}
