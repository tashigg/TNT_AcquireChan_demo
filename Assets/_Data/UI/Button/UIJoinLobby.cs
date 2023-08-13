using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIJoinLobby : BaseButton
{
    protected override void OnClick()
    {
        LobbyManager.Instance.JoinLobby();
    }
}
