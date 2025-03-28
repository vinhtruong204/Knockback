using Unity.Netcode;
using UnityEngine;

public class PlayerTeamId : NetworkBehaviour
{
    private NetworkVariable<int> teamId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public int TeamId => teamId.Value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            teamId.Value = (int)OwnerClientId;
    }
}