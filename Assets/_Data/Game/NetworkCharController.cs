using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkCharController : SaiMonoBehaviour
{
    [Header("Default")]
    [SerializeField] protected float m_WalkSpeed = 2.0f;
    [SerializeField] protected float m_RunSpeed = 7f;
    [SerializeField] protected float m_RotateSpeed = 8.0f;
    [SerializeField] protected float m_JumpForce = 70.0f;

    [SerializeField] protected Rigidbody m_RigidBody = null;
    [SerializeField] protected Animator m_Animator = null;
    [SerializeField] protected float m_MoveTime = 0;
    [SerializeField] protected float m_MoveSpeed = 0.0f;

    [Header("Network")]
    public NetworkObject networkObject;
    public PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        m_MoveSpeed = m_WalkSpeed;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadRigibody();
        this.LoadAnimator();
        this.LoadNetworkObject();
        this.LoadPlayerData();
    }

    private void Update()
    {
        this.Moving();
    }

    protected virtual void LoadRigibody()
    {
        if (this.m_RigidBody != null) return;
        this.m_RigidBody = GetComponentInChildren<Rigidbody>();
        Debug.LogWarning(transform.name + ": LoadRigibody", gameObject);
    }

    protected virtual void LoadAnimator()
    {
        if (this.m_Animator != null) return;
        this.m_Animator = GetComponentInChildren<Animator>();
        Debug.LogWarning(transform.name + ": LoadAnimator", gameObject);
    }

    protected virtual void LoadNetworkObject()
    {
        if (this.networkObject != null) return;
        this.networkObject = GetComponent<NetworkObject>();
        Debug.LogWarning(transform.name + ": LoadNetworkObject", gameObject);
    }

    protected virtual void LoadPlayerData()
    {
        if (this.playerData != null) return;
        this.playerData = GetComponentInChildren<PlayerData>();
        Debug.LogWarning(transform.name + ": LoadPlayerData", gameObject);
    }

    protected virtual void Moving()
    {
        if (!this.playerData.IsServer) return;
        float h = this.playerData.horizontal.Value;
        float v = this.playerData.vertical.Value;
        bool isRun = this.playerData.isRun.Value;

        // input
        Vector3 vel = m_RigidBody.velocity;

        bool isMove = ((0 != h) || (0 != v));

        m_MoveTime = isMove ? (m_MoveTime + Time.deltaTime) : 0;
        //bool isRun = (m_RunningStart <= m_MoveTime);

        // move speed (walk / run)
        float moveSpeed = isRun ? m_RunSpeed : m_WalkSpeed;
        m_MoveSpeed = isMove ? Mathf.Lerp(m_MoveSpeed, moveSpeed, (8.0f * Time.deltaTime)) : m_WalkSpeed;

        Vector3 inputDir = new Vector3(h, 0, v);
        if (1.0f < inputDir.magnitude) inputDir.Normalize();

        if (0 != h) vel.x = (inputDir.x * m_MoveSpeed);
        if (0 != v) vel.z = (inputDir.z * m_MoveSpeed);

        m_RigidBody.velocity = vel;

        if (isMove)
        {
            // rotation
            float t = (m_RotateSpeed * Time.deltaTime);
            Vector3 forward = Vector3.Slerp(this.transform.forward, inputDir, t);
            this.transform.rotation = Quaternion.LookRotation(forward);
        }

        m_Animator.SetBool("isMove", isMove);
        m_Animator.SetBool("isRun", isRun);
    }
}
