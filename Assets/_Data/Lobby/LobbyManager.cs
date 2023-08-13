using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    public LobbyEvents lobbyEvents;
    public TMP_InputField inputLobbyName;
    public TMP_InputField inputMaxPlayer;
    public TMP_InputField inputLobbyCode;
    public TMP_InputField inputPlayerCount;
    public TextMeshProUGUI txtTextLobbyId;
    public string profileName;
    public string lobbyName;
    public string lobbyCode;
    public string lobbyId;
    public bool isHost = false;
    public int maxPlayer;
    public int countPlayer;
    public Lobby lobby;

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);

        this.ServiceInit();
    }

    protected override void Start()
    {
        this.RandomLobbyName();
        InvokeRepeating(nameof(LobbyUpdate), 2f, 1f);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadLobbyEvents();
        this.LoadInputLobbyName();
        this.LoadInputMaxPlayer();
        this.LoadInputLobbyCode();
        this.LoadInputPlayerCount();
        this.LoadTextLobbyId();
    }

    protected virtual void LoadLobbyEvents()
    {
        if (this.lobbyEvents != null) return;
        this.lobbyEvents = GameObject.Find("LobbyEvents").GetComponent<LobbyEvents>();
        Debug.LogWarning(transform.name + ": LoadLobbyEvents", gameObject);
    }

    protected virtual void LoadInputLobbyName()
    {
        if (this.inputLobbyName != null) return;
        this.inputLobbyName = GameObject.Find("InputLobbyName").GetComponent<TMP_InputField>();
        Debug.LogWarning(transform.name + ": LoadInputLobbyName", gameObject);
    }

    protected virtual void LoadTextLobbyId()
    {
        if (this.txtTextLobbyId != null) return;
        this.txtTextLobbyId = GameObject.Find("TextLobbyId").GetComponent<TextMeshProUGUI>();
        Debug.LogWarning(transform.name + ": LoadTextLobbyId", gameObject);
    }

    protected virtual void LoadInputMaxPlayer()
    {
        if (this.inputMaxPlayer != null) return;
        this.inputMaxPlayer = GameObject.Find("InputMaxPlayer").GetComponent<TMP_InputField>();
        Debug.LogWarning(transform.name + ": LoadInputMaxPlayer", gameObject);
    }

    protected virtual void LoadInputLobbyCode()
    {
        if (this.inputLobbyCode != null) return;
        this.inputLobbyCode = GameObject.Find("InputLobbyCode").GetComponent<TMP_InputField>();
        Debug.LogWarning(transform.name + ": LoadInputLobbyCode", gameObject);
    }

    protected virtual void LoadInputPlayerCount()
    {
        if (this.inputPlayerCount != null) return;
        this.inputPlayerCount = GameObject.Find("InputPlayerCount").GetComponent<TMP_InputField>();
        Debug.LogWarning(transform.name + ": LoadInputPlayerCount", gameObject);
    }

    public virtual async void CreateLobby()
    {
        this.lobbyName = this.inputLobbyName.text;
        this.maxPlayer = int.Parse(this.inputMaxPlayer.text);

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            IsPrivate = false
        };
        this.lobby = await LobbyService.Instance.CreateLobbyAsync(this.lobbyName, this.maxPlayer, options);
        this.lobbyCode = this.lobby.LobbyCode;
        this.lobbyId = this.lobby.Id;
        this.isHost = true;
        this.inputLobbyCode.text = this.lobbyCode;
        this.txtTextLobbyId.text = this.lobbyId;
        this.lobbyEvents.SubscribeToLobbyEvents();
    }

    public virtual async void JoinLobby()
    {
        this.lobbyCode = this.inputLobbyCode.text;
        this.lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(this.lobbyCode);
        this.lobbyId = this.lobby.Id;
        this.lobbyName= this.lobby.Name;
        this.txtTextLobbyId.text = this.lobbyId;
        this.lobbyEvents.SubscribeToLobbyEvents();
    }

    protected virtual void RandomLobbyName()
    {
        this.inputLobbyName.text = Random.Range(1111111, 10001000).ToString();
    }

    protected virtual async void ServiceInit()
    {
        this.profileName = "profile_"+Random.Range(1111111, 10001000).ToString();
        Debug.Log("ServiceInit: " + this.profileName);
        var options = new InitializationOptions();
        options.SetProfile(this.profileName);
        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    protected virtual void LobbyUpdate()
    {
        if (this.lobby == null) return;
        Debug.Log("LobbyUpdate");
        this.countPlayer = this.lobby.Players.Count;
        this.maxPlayer = this.lobby.MaxPlayers;
        this.inputPlayerCount.text = this.countPlayer.ToString();
        this.inputMaxPlayer.text = this.maxPlayer.ToString();
    }

    public virtual bool IsReady()
    {
        return this.lobbyId != ""; 
    }
}
