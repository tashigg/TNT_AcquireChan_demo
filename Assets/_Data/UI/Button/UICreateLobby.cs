using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICreateLobby : BaseButton
{
    protected override void OnClick()
    {
        LobbyManager.Instance.CreateLobby();
    }
}
