using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;

		[SerializeField] float m_JumpPower;
		private const float JumpPower_FOR_FOOT = 6f;
		private const float JumpPower_FOR_EXOSKELETON = 6f;

		private float lastJumpPressedAt;
		private const float EXOSKELETON_DOUBLE_JUMP_PRESS_TIME = 0.5f;

		private float jumpLandingTime;

		private float lastJumpedTime;
		private const float JUMP_ALLOWED_FREQUENCY = 0.3f;

		// if the player has landed few moments ago, his jumping speed should be increased
		// this variable determines the timeframe for that
		private const float EXOSKELETON_DOUBLE_JUMP_TIME = 0.5f;

		private bool justLandedFromJump = false;

		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others

		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		private const float MoveSpeedMultiplier_FOR_FOOT = 1f;
		private const float MoveSpeedMultiplier_FOR_EXOSKELETON = 1.5f;

		[SerializeField] float m_AnimSpeedMultiplier = 1f;

		[SerializeField] float m_GroundCheckDistance = 0.1f;
		private const float GroundCheckDistance_FOR_FOOT = 0.3f;
		private const float GroundCheckDistance_FOR_EXOSKELETON = 0.9f;

		private const float COLLIDER_CENTER_FOR_FOOT = 1.08f;
		private const float COLLIDER_HEIGHT_FOR_FOOT = 2.16f;

		private const float COLLIDER_CENTER_FOR_EXOSKELETON = 0.87f;
		private const float COLLIDER_HEIGHT_FOR_EXOSKELETON = 2.4f;

		[SerializeField] bool m_HasExoskeleton = false;

		[SerializeField] GameObject m_ExoskeletonLeft;
		[SerializeField] GameObject m_ExoskeletonRight;

		[SerializeField] PhysicMaterial m_PhysicsMaterialForFoot;
		[SerializeField] PhysicMaterial m_PhysicsMaterialForExoskeleton;

		private bool hasJumped = false;

		[SerializeField] bool m_HasJetPack = false;
		[SerializeField] float m_JetPackForce = 15f;
		[SerializeField] AudioSource m_JetPackAudioSource;
		[SerializeField] ParticleSystem[] m_JetPackParticles;
		[SerializeField] GameObject m_JetPack;
		private const int JETPACK_LAYER_INDEX = 1;
		private const float JETPACK_TOP_REACH = 40;
		private const float JETPACK_MIN_VOLUME = 0.5f;
		private const float JETPACK_MIN_DOWNWARD_VELOCITY = -5f;

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

		private float lastRunClipPlayedAt;
		public AudioClip runClip;
		public AudioClip jumpClip;
		public AudioClip landClip;

		public AudioClip runClipExosKeleton;
		public AudioClip jumpClipExosKeleton;
		public AudioClip landClipExosKeleton;

		public static ThirdPersonCharacter Instance;

		void Start()
		{
			Instance = this;

			m_Animator = GetComponent<Animator>();
			m_AudioSource = GetComponent<AudioSource>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;

			setJetPack(m_HasJetPack, true);
			m_JetPackAudioSource.volume = JETPACK_MIN_VOLUME;

			setExoskeleton(m_HasExoskeleton, true);
		}

		public bool hasExoskeleton()
		{
			return m_HasExoskeleton;
		}

		public void setExoskeleton(bool val, bool force = false)
		{
			if(m_IsGrounded == false)
			{
				if(!force) 
				{
					Debug.LogError("Player needs to be grounded before toggling Exoskeleton");
					return;
				}
			}

			if(val)
			{
				m_HasExoskeleton = true;

				m_Capsule.height = COLLIDER_HEIGHT_FOR_EXOSKELETON;
				m_Capsule.center = new Vector3(m_Capsule.center.x, COLLIDER_CENTER_FOR_EXOSKELETON, m_Capsule.center.z);

				m_GroundCheckDistance = GroundCheckDistance_FOR_EXOSKELETON;

				m_JumpPower = JumpPower_FOR_EXOSKELETON;

				m_MoveSpeedMultiplier = MoveSpeedMultiplier_FOR_EXOSKELETON;

				m_Capsule.material = m_PhysicsMaterialForExoskeleton;

				setJetPack(false, true);
			}
			else
			{
				m_HasExoskeleton = false;

				m_Capsule.height = COLLIDER_HEIGHT_FOR_FOOT;
				m_Capsule.center = new Vector3(m_Capsule.center.x, COLLIDER_CENTER_FOR_FOOT, m_Capsule.center.z);

				m_GroundCheckDistance = GroundCheckDistance_FOR_FOOT;

				m_JumpPower = JumpPower_FOR_FOOT;

				m_MoveSpeedMultiplier = MoveSpeedMultiplier_FOR_FOOT;

				m_Capsule.material = m_PhysicsMaterialForFoot;
			}

			m_ExoskeletonLeft.SetActive(val);
			m_ExoskeletonRight.SetActive(val);

			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}

		public bool hasJetPack()
		{
			return m_HasJetPack;
		}

		public void setJetPack(bool val, bool force = false)
		{
			if(m_IsGrounded == false)
			{
				if(!force) 
				{
					Debug.LogError("Player needs to be grounded before toggling JetPack");
					return;
				}
			}

			if(val)
			{
				m_HasJetPack = true;
				m_Animator.SetLayerWeight(JETPACK_LAYER_INDEX, 1);

				setExoskeleton(false, true);
			}
			else
			{
				m_HasJetPack = false;
				m_Animator.SetLayerWeight(JETPACK_LAYER_INDEX, 0);
			}

			m_JetPack.SetActive(val);
		}

		public void addJetPackForce()
		{
			if(m_HasJetPack && transform.position.y < JETPACK_TOP_REACH)
			{
				hasJumped = true;
				// remove all his previous force
				m_Rigidbody.velocity = Vector3.zero;

				m_Rigidbody.AddForce(Vector3.up * m_JetPackForce, ForceMode.Impulse);
				m_JetPackAudioSource.Play();

				if(m_JetPackAudioSource.volume == JETPACK_MIN_VOLUME)
				{
					m_JetPackAudioSource.volume = 1;
					StartCoroutine("reduceJetSound");
				}
				else 
				{
					m_JetPackAudioSource.volume = 1;
				}

				foreach(ParticleSystem ps in m_JetPackParticles)
				{
					ps.Play();
					ps.startSpeed = 10f;
				}

				if(IsInvoking("reduceJetBurne"))
				{
					CancelInvoke("reduceJetBurne");
				}

				Invoke("reduceJetBurne", 0.5f);
			}
		}

		void reduceJetBurne()
		{
			foreach(ParticleSystem ps in m_JetPackParticles)
			{
				ps.startSpeed = 3f;
			}
		}

		IEnumerator reduceJetSound()
		{
			while(m_JetPackAudioSource.volume > JETPACK_MIN_VOLUME)
			{
				m_JetPackAudioSource.volume = Mathf.Lerp(m_JetPackAudioSource.volume, JETPACK_MIN_VOLUME, Time.deltaTime * 3f);
				yield return null;
			}

			m_JetPackAudioSource.volume = JETPACK_MIN_VOLUME;
		}

		public void Move(Vector3 move, bool crouch, bool jump)
		{

			if(Input.GetKeyDown(KeyCode.LeftCommand))
			{
				m_MoveSpeedMultiplier = 3;
			}
			else if(Input.GetKeyUp(KeyCode.LeftCommand))
			{
				m_MoveSpeedMultiplier = 1;
			}

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			//Debug.Log(move);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			//Debug.Log(m_TurnAmount);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			if(jump)
			{
				lastJumpPressedAt = Time.time;
			}


			if(justLandedFromJump && m_IsGrounded && hasExoskeleton() && lastJumpPressedAt > Time.time - EXOSKELETON_DOUBLE_JUMP_PRESS_TIME)
			{
				// if the player has pressed jump button few moments ago when in air. allow it to work now that he is on ground
				jump = true;
				//Debug.Log("Delayed Jump");
			}

			justLandedFromJump = false;

			// control and velocity handling is different when grounded and airborne:
			if(jump && m_HasJetPack)
			{
				addJetPackForce();
				hasJumped = true;
			}
			else if (m_IsGrounded && !hasJumped)
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
			UpdateAnimator(move);

			if(m_HasJetPack)
			{
				if(hasJumped)
				{
					// player has a jetpack and he has jumped already

					transform.Translate(Vector3.forward * m_ForwardAmount * m_JetPackForce * 2f * Time.deltaTime);
					m_Animator.SetLayerWeight(2, 1);

					//Debug.Log(m_Rigidbody.velocity);

					// rotate the player forward a bit when he is using the jet and moving forward
					Vector3 rotation = new Vector3(move.z == 0 ? 0 : Mathf.Clamp(15f / move.z, -15, 15) , 0, 0);
					//Debug.Log("Move: " + move);
					//Debug.Log(rotation);
					m_CharacterRoot.localRotation = Quaternion.Lerp(m_CharacterRoot.localRotation, Quaternion.Euler(rotation), Time.deltaTime * 10) ;
				}
				else
				{
					m_Animator.SetLayerWeight(2, 0);	
					m_CharacterRoot.localRotation = Quaternion.Euler(Vector3.zero);
				}
			}


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


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);

			if(hasExoskeleton())
			{
				// when player is in idle mode players leg should not move while using Exoskeleton
				if(m_IsGrounded && m_ForwardAmount == 0 && m_TurnAmount == 0)
				{
					m_Animator.SetLayerWeight(3, 1);
				}
				else
				{
					m_Animator.SetLayerWeight(3, 0);
				}
			}

			if (!m_IsGrounded && !m_HasJetPack)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded)
			{
				m_Animator.SetFloat("JumpLeg", jumpLeg);
			}


			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;

			if(m_HasJetPack && hasJumped)
			{
				// if player has jetpack, he should not have too much downward velocity
				if(m_Rigidbody.velocity.y < JETPACK_MIN_DOWNWARD_VELOCITY) m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, JETPACK_MIN_DOWNWARD_VELOCITY, m_Rigidbody.velocity.z);	
			}
		}


		void HandleGroundedMovement(bool crouch, bool jump, Vector3 moveDirection)
		{
			// check whether conditions are right to allow a jump:

			if (jump && !crouch 
				&& lastJumpedTime < Time.time - JUMP_ALLOWED_FREQUENCY
				&& (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") 
					|| (hasExoskeleton() && lastJumpPressedAt > Time.time - EXOSKELETON_DOUBLE_JUMP_PRESS_TIME )
				)
			)
			{
				// jump!
				//m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, hasExoskeleton() && jumpLandingTime > Time.time - EXOSKELETON_DOUBLE_JUMP_TIME ? m_JumpPower * 1.25f : m_JumpPower, m_Rigidbody.velocity.z);
				moveDirection = transform.TransformDirection(moveDirection);
				//Debug.Log("Direction: " + moveDirection);
				//Debug.Log("Jumped");
				lastJumpPressedAt = 0;
				lastJumpedTime = Time.time;

				float maxHorizontalVelocity = 6.67f * m_MoveSpeedMultiplier;

				Vector3 velocity = new Vector3( maxHorizontalVelocity * moveDirection.x, 
					hasExoskeleton() && jumpLandingTime > Time.time - EXOSKELETON_DOUBLE_JUMP_TIME ? m_JumpPower * 1.25f : m_JumpPower, 
					maxHorizontalVelocity * moveDirection.z
				);

				m_Rigidbody.velocity = velocity;

				//Debug.Log("Velocity: " + m_Rigidbody.velocity);

				/*
				m_Rigidbody.AddForce(new Vector3(moveDirection.x, 
					(hasExoskeleton() && jumpLandingTime > Time.time - EXOSKELETON_DOUBLE_JUMP_TIME ? 2f : 1.5f), 
					moveDirection.z) 
					* m_JumpPower, ForceMode.VelocityChange);
				*/
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;

				m_AudioSource.PlayOneShot(hasExoskeleton() ? jumpClipExosKeleton : jumpClip);

				hasJumped = true;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}

		public void footPlaced()
		{
			if(Time.time - lastRunClipPlayedAt > 0.1f)
			{
				lastRunClipPlayedAt = Time.time;
				m_AudioSource.PlayOneShot(hasExoskeleton() ? runClipExosKeleton : runClip, m_ForwardAmount / 3f);	
			}
		}

		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && hasJumped == false && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
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
					m_AudioSource.PlayOneShot(hasExoskeleton() ? landClipExosKeleton : landClip, Mathf.Clamp01( Mathf.Abs(m_Rigidbody.velocity.y) / 7f) );

					jumpLandingTime = Time.time;

					hasJumped = false;

					m_JetPackAudioSource.Stop();

					foreach(ParticleSystem ps in m_JetPackParticles)
					{
						ps.Stop();
					}

					justLandedFromJump = true;
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
