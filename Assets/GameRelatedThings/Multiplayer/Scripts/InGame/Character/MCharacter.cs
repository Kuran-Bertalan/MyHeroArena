using UnityEngine;
using System.Collections;

public class MCharacter : MonoBehaviour
{
	public  MBattleManager battleManager;
	public  MBattleManagerUI battleManagerUI;

	private float idleSwitchDelay;
	private AudioClip currentAudioClip;
	public  AudioClip attackSFX;

	public  MGlobal.CharacterState state;

	public  MGlobal.AttackType attackType;
	public  GameObject projectile;
	private MCharacter currentTarget;
	private bool isMovingToTarget = false;
	private float attackDelay;

	private Vector3 initialPosition;
	private Quaternion initialRotation;
	public  float moveToInitialPositionSpeed = 15.0F;
	private bool isMovingToInitialPosition = false;
	private float movingToInitialPositionDelay;

	public  GameObject characterGameObject;
	public  int id;
	public  string name;

	private CharacterController controller;
	private Animator mAnimator;

	private bool isAttacking = false;
	public  Skill[] skills;
	private Skill currentSkill;
	private bool isUsingSkill = false;
	private float skillAttackDelay;

	// Character Stats
	public int level;
	public int health;
	public int mana;
	public int attack;
	public int armor;
	public int actionSpeed;

	private int currentHealth;
	private int currentMana;
	private int currentAttack;
	private int currentArmor;
	private int currentActionSpeed;

	void Start()
	{
		controller = transform.GetComponent<CharacterController>();

		currentHealth = health;
		currentMana = mana;
		currentAttack = attack;
		currentArmor = armor;
		currentActionSpeed = actionSpeed;

		initialPosition = transform.position;
		initialRotation = transform.rotation;

		mAnimator = GetComponent<Animator>();
	}

	void Update()
	{
		if (state == MGlobal.CharacterState.Dead) return;

		if (isMovingToTarget)
		{
			if (Vector3.Distance(currentTarget.transform.position, transform.position) < 3 * transform.localScale.x)
			{
				isMovingToTarget = false;
				if (isAttacking)
				{
					//attackDelay = monsterAnimator.Length("PhysicalAttack") - 0.6F + Time.time;
					mAnimator.SetTrigger("TrBasicAttack");

					//attackDelay = 1F + Time.time;

					if (attackSFX != null)
					{
						currentAudioClip = attackSFX;
						//Invoke("PlayAudioClip", monsterAnimator.Length("PhysicalAttack") - 0.6F);
					}

					//monsterAnimator.PlayQueued("PhysicalAttack", new string[] { "MoveBackward", "Idle" });
				}
				else
				{
					//specialAttackDelay = monsterAnimator.Length("SpecialAttack") - 0.6F + Time.time;

					//monsterAnimator.PlayQueued("SpecialAttack", new string[] { "MoveBackward", "Idle" });
				}
			}
			else
			{
				MoveTo(currentTarget.transform.position);
				mAnimator.SetTrigger("TrWalkBackward");
			}
		}

		if (isAttacking && attackDelay < Time.time && !isMovingToTarget)
		{
			currentTarget.Damage(currentAttack, currentTarget);

			isAttacking = false;

			movingToInitialPositionDelay = Time.time + 1;
			isMovingToInitialPosition = true;
		}

		if (isUsingSkill && skillAttackDelay < Time.time && !isMovingToTarget)
		{
			if (currentSkill.effectObject != null)
			{
				//InstantiateParticlesOver(currentSkill.targets, currentSkill.effectObject, currentSkill.effectObjectPosition);
			}

			if (currentSkill.target == Global.Targets.Ally || currentSkill.target == Global.Targets.Enemy)
			{
				DamageBySkill(currentSkill, currentTarget);
			}

			currentMana -= currentSkill.manaCost;

			isUsingSkill = false;

			movingToInitialPositionDelay = Time.time + 1;
			isMovingToInitialPosition = true;
		}

		if (isMovingToInitialPosition && movingToInitialPositionDelay < Time.time)
		{
			if (Vector3.Distance(initialPosition, transform.position) < 0.1)
			{
				isMovingToInitialPosition = false;
			}
			else
			{
				MoveToInitialPosition();
			}
		}
	}

	public void Attack(MCharacter target)
	{
		// Rotate to target
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 4 * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

		if (attackType == MGlobal.AttackType.Melee)
		{
			isMovingToTarget = true;
			mAnimator.SetTrigger("TrWalkForward");
		}
		else
		{
			isMovingToTarget = false;

			if (projectile != null)
			{
				GameObject projectileClone = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
				Projectile projectileCloneScript = (Projectile)projectileClone.GetComponent("Projectile");
				projectileCloneScript.target = target.transform;

				attackDelay = Vector3.Distance(target.transform.position, transform.position) / projectileCloneScript.speed + Time.time;
			}
			else
			{
				//attackDelay = mAnimator.SetTrigger("TrHitWithSword") - 0.6F + Time.time;
				mAnimator.SetTrigger("TrBasicAttack");

			}
			//monsterAnimator.PlayQueued("SpecialAttack");
		}

		isAttacking = true;
		currentTarget = target;
	}

	public void UseSkill(int skillNumber, MCharacter target)
	{
		currentSkill = skills[skillNumber];

		// Rotate to target
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 4 * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

		if (currentSkill.attackType == Global.AttackType.Melee)
		{
			isMovingToTarget = true;
		}
		else
		{
			isMovingToTarget = false;

			if (currentSkill.projectile != null)
			{
				GameObject projectileClone = Instantiate(currentSkill.projectile, transform.position, transform.rotation) as GameObject;
				Projectile projectileCloneScript = (Projectile)projectileClone.GetComponent("Projectile");
				projectileCloneScript.target = target.transform;

				skillAttackDelay = Vector3.Distance(target.transform.position, transform.position) / projectileCloneScript.speed + Time.time;
			}
			else
			{
				//specialAttackDelay = monsterAnimator.Length("SpecialAttack") - 0.6F + Time.time;
			}

			//monsterAnimator.PlayQueued("SpecialAttack");
		}

		isUsingSkill = true;
		currentTarget = target;
	}

	public void Damage(int attackValue, MCharacter currentTarget)
	{
		int damage = TakeDamage(attackValue, currentArmor);
		DamageTheTarget(damage);


		if (currentTarget.tag == "Enemy")
		{
			battleManagerUI.updateEnemyHealth(currentHealth, currentTarget.id);
		}
		else
		{
			Debug.Log(currentTarget.name);
			battleManagerUI.updatePlayerCharactersHealth(currentHealth, currentTarget.id);
		}
	}

	private void DamageTheTarget(int damage)
	{
		if (currentHealth - damage > 0)
		{
			currentHealth -= damage;
			mAnimator.SetTrigger("TrGetDamage");
			//monsterAnimator.PlayQueued("TakeDamage");
		}
		else
		{
			currentHealth = 0;
			state = MGlobal.CharacterState.Dead;
			mAnimator.SetTrigger("TrDying");
			//monsterAnimator.PlayDead();
		}

		//battleManagerUI.DisplayDamage(damage, GetTopPosition(), Color.white);
	}


	public void DamageBySkill(Skill skill, MCharacter target)
	{
		target.Damage(currentAttack, target);
	}

	public int TakeDamage(int attackValue, int defenseValue)
	{
		return attackValue - defenseValue;
	}

	public void MoveTo(Vector3 targetPosition)
	{
		if (transform.position != targetPosition)
		{
			Vector3 dir = targetPosition - transform.position;
			float dist = dir.magnitude;
			float move = 5 * transform.localScale.x * Time.deltaTime;


			if (dist > move)
			{
				controller.Move(dir.normalized * move);
			}
			else
			{
				controller.Move(dir);
			}

			// Rotate to target
			dir.y = 0;
			transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 2 * transform.localScale.x);
		}
	}

	public void MoveToInitialPosition()
	{
		Vector3 dir = initialPosition - transform.position;
		float dist = dir.magnitude;
		float move = moveToInitialPositionSpeed * transform.localScale.x * Time.deltaTime;


		if (dist > move)
		{
			controller.Move(dir.normalized * move);
		}
		else
		{
			controller.Move(dir);
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.time * 0.008F * transform.localScale.x);
	}

	public Vector3 GetTopPosition()
	{
		Vector3 characterTopPosition = transform.position;
		Renderer characterRenderer = GetComponentInChildren<Renderer>();
		characterTopPosition.y += characterRenderer.bounds.size.y;

		return characterTopPosition;
	}

	public Vector3 GetCenterPosition()
	{
		Vector3 characterTopPosition = transform.position;
		Renderer monsterRenderer = GetComponentInChildren<Renderer>();
		characterTopPosition.y += monsterRenderer.bounds.size.y / 2;

		return characterTopPosition;
	}

	public Vector3 GetBottomPosition()
	{
		return transform.position;
	}

	public Vector3 GetPosition(MGlobal.Position position)
	{
		if (position == MGlobal.Position.Top)
		{
			return GetTopPosition();
		}
		else if (position == MGlobal.Position.Center)
		{
			return GetCenterPosition();
		}
		else if (position == MGlobal.Position.Bottom)
		{
			return GetBottomPosition();
		}

		return transform.position;
	}

	public void InstantiateParticlesOver(MGlobal.Targets Target, GameObject particleObject, MGlobal.Position position)
	{
		if (Target == MGlobal.Targets.Ally || Target == MGlobal.Targets.Enemy)
		{
			GameObject particleObjectClone = Instantiate(particleObject, currentTarget.GetPosition(position), currentTarget.transform.rotation) as GameObject;
		}
	}

	public void PlayAudioClip()
	{
		GetComponent<AudioSource>().PlayOneShot(currentAudioClip, 1f);
	}

	#region Getters

	public int GetLevel()
	{
		return level;
	}

	public int GetCurrentHealth()
	{
		return currentHealth;
	}

	public int GetMaximumHealth()
	{
		return health;
	}

	public int GetCurrentMana()
	{
		return currentMana;
	}

	public int GetMaximumMana()
	{
		return mana;
	}

	public int GetAttack()
	{
		return currentAttack;
	}

	public int GetDefense()
	{
		return currentArmor;
	}

	public int GetSpeed()
	{
		return currentActionSpeed;
	}

	public bool IsAttacking()
	{
		return isAttacking;
	}

	public bool IsUsingSkill()
	{
		return isUsingSkill;
	}

	public bool IsMovingToTarget()
	{
		return isMovingToTarget;
	}

	public bool IsMovingToInitialPosition()
	{
		return isMovingToInitialPosition;
	}
	#endregion
}
