using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Grenade : NetworkBehaviour, IDisableAfterTime
{
    private float timeToReturnPoolMax = 2f;
    private float force = 5f;
    private Rigidbody2D grenadeRigidbody;

    private int teamId;

    private void Awake()
    {
        grenadeRigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        // Add force to the grenade in the direction it is facing
        if (transform.localScale.x < 0f)
            grenadeRigidbody.AddForce(Vector2.left * force, ForceMode2D.Impulse);
        else if (transform.localScale.x > 0f)
            grenadeRigidbody.AddForce(Vector2.right * force, ForceMode2D.Impulse);

        StartCoroutine(DisableAfterTime());
    }

    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(timeToReturnPoolMax);

        if (IsServer) GetComponent<NetworkObject>().Despawn();
    }

    public void Initialize(int teamId, Vector3 scale)
    {
        this.teamId = teamId;
        transform.localScale = scale;
    }
}
