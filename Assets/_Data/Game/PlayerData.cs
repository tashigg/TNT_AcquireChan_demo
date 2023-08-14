using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<float> horizontal = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> vertical = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isRun = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Update()
    {
        this.OwnerUpdateInput();
    }

    protected virtual void OwnerUpdateInput()
    {
        if (!this.IsOwner) return;
        this.horizontal.Value = Input.GetAxis("Horizontal");
        this.vertical.Value = Input.GetAxis("Vertical");
        this.isRun.Value = Input.GetKey(KeyCode.LeftShift);
    }
}
