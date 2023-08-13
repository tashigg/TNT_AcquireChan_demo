using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartGame : BaseButton
{
    protected override void OnClick()
    {
        if (!LobbyManager.Instance.IsReady()) return;
        Debug.Log("Start Game");
        SceneManager.LoadScene("2_game");
    }
}
