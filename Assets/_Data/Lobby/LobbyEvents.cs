using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbyEvents : SaiMonoBehaviour
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

    public virtual void SubscribeToLobbyEvents()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        Lobbies.Instance.SubscribeToLobbyEventsAsync(this.lobbyManager.lobbyId, callbacks);
    }

    protected virtual void OnLobbyChanged(ILobbyChanges changes)
    {
        changes.ApplyToLobby(this.lobbyManager.lobby);
    }
}
