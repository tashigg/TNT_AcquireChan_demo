using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkCharController : SaiMonoBehaviour
{
	[Header("Default")]
	[SerializeField] private float	m_WalkSpeed		= 2.0f;
	[SerializeField] private float	m_RunSpeed		= 7f;
	[SerializeField] private float	m_RotateSpeed	= 8.0f;
	[SerializeField] private float	m_JumpForce		= 70.0f;

	[SerializeField] private Rigidbody	m_RigidBody	= null;
	[SerializeField] private Animator	m_Animator	= null;
	[SerializeField] private float		m_MoveTime	= 0;
	[SerializeField] private float		m_MoveSpeed	= 0.0f;
	[SerializeField] private bool		m_IsGround	= true;

	[Header("Network")]
	public NetworkObject networkObject;


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

	protected virtual void Moving()
    {
		if (!this.networkObject.IsOwner) return;
		if (null == m_RigidBody) return;
		if (null == m_Animator) return;

		// check ground
		float rayDistance = 0.3f;
		Vector3 rayOrigin = (this.transform.position + (Vector3.up * rayDistance * 0.5f));
		bool ground = Physics.Raycast(rayOrigin, Vector3.down, rayDistance, LayerMask.GetMask("Default"));
		if (ground != m_IsGround)
		{
			m_IsGround = ground;

			// landing
			if (m_IsGround)
			{
				m_Animator.Play("landing");
			}
		}

		// input
		Vector3 vel = m_RigidBody.velocity;
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		bool isMove = ((0 != h) || (0 != v));

		m_MoveTime = isMove ? (m_MoveTime + Time.deltaTime) : 0;
		//bool isRun = (m_RunningStart <= m_MoveTime);
		bool isRun = Input.GetKey(KeyCode.LeftShift);

		// move speed (walk / run)
		float moveSpeed = isRun ? m_RunSpeed : m_WalkSpeed;
		m_MoveSpeed = isMove ? Mathf.Lerp(m_MoveSpeed, moveSpeed, (8.0f * Time.deltaTime)) : m_WalkSpeed;
		//		m_MoveSpeed = moveSpeed;

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


		// jump
		if (Input.GetButtonDown("Jump") && m_IsGround)
		{
			m_Animator.Play("jump");
			m_RigidBody.AddForce(Vector3.up * m_JumpForce);
		}
	}
}
