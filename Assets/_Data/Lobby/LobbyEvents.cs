using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbyEvents : LobbyAbstract
{
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
