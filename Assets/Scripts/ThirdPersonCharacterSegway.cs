using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacterSegway : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 90;
		[SerializeField] float m_StationaryTurnSpeed = 90;

		[SerializeField] float m_JumpPower;


		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;


		[SerializeField] float m_GroundCheckDistance = 0.1f;

		private bool hasJumped = false;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		AudioSource m_AudioSource;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;

		[SerializeField] Transform m_CharacterRoot;

		public float acceleration = 50;
		public float topSpeed = 10;

		public AudioSource secondAudioSource;

		[SerializeField] AudioClip jumpClip;
		[SerializeField] AudioClip landClip;

		private float lastRunClipPlayedAt;

		public static ThirdPersonCharacterSegway Instance;

		void Start()
		{
			Instance = this;

			m_Animator = GetComponent<Animator>();
			m_AudioSource = GetComponent<AudioSource>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;

			show();
		}

		public void show()
		{
			Debug.Log("Segway Enabled");
			gameObject.SetActive(true);
			if(m_Rigidbody != null)
			{
				m_Rigidbody.velocity = Vector3.zero;	
			}
			PlayerController.TogglePlayerTransform(transform);
		}

		public void hide()
		{
			gameObject.SetActive(false);
		}

		public void Move(Vector3 move, bool crouch, bool jump)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			//move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			//move = Vector3.ProjectOnPlane(move, m_GroundNormal);

			if(move.z != -1)
			{
				m_TurnAmount = Mathf.Atan2(move.x, move.z);	

			}

			m_TurnAmount = move.x;

			/*
			if(move.magnitude == 1)
			{
				Debug.Log("Move" + move);

				Debug.Log("Turn" + m_TurnAmount);	
			}
			*/

			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();



			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded && !hasJumped)
			{
				HandleGroundedMovement(crouch, jump, move);
			}
			else
			{
				HandleAirborneMovement();
			}

			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			//UpdateAnimator(move);

			m_AudioSource.volume = move.magnitude;
		}


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}



		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump, Vector3 moveDirection)
		{
			// check whether conditions are right to allow a jump:

			if (jump && !crouch)
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);

				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;

				secondAudioSource.PlayOneShot(jumpClip);

				hasJumped = true;
			}
			else
			{
				//moveDirection = transform.TransformDirection(moveDirection);

				if(m_Rigidbody.velocity.magnitude > topSpeed)
				{
					// speed is already a lot, don't give any more speed
					/*
					Vector3 velocity = new Vector3( maxHorizontalVelocity * moveDirection.x, 
						m_Rigidbody.velocity.y, 
						maxHorizontalVelocity * moveDirection.z
					);

					m_Rigidbody.velocity = velocity;
					*/
				}
				else
				{
					// top speed is not reached yet
					m_Rigidbody.AddForce(transform.forward * acceleration * moveDirection.z, ForceMode.Acceleration);
				}
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			if(m_IsGrounded)
			{
				float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
				transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);	
			}
		}


		public void OnAnimatorMove()
		{
			
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				if(m_IsGrounded == false)
				{
					// player has just landed
					//Debug.Log("Hit Ground");
					//Debug.Log("Volume: " + Mathf.Clamp01( Mathf.Abs(m_Rigidbody.velocity.y) / 7f));
					secondAudioSource.PlayOneShot(landClip, Mathf.Clamp01( Mathf.Abs(m_Rigidbody.velocity.y) / 7f) );

					hasJumped = false;

				}
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
