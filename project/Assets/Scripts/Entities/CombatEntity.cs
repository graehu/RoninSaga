using UnityEngine;
using System.Collections.Generic;

public class CombatEntity : Entity 
{
	#region static variables

	public static List<CombatEntity> activeCombatEntities = new List<CombatEntity>();

	#endregion

	#region public variables

	public float knockbackScale = 1.0f;

	public MeleeAttack meleeAreaDamage;
	
	public Projectile magicPrefab;

	#endregion

	#region protected variables

	#endregion

	#region private variables
	 
	float knockBackTime = 0;
	Vector3 knockBackVelocity = Vector3.zero;

	#endregion

	#region public methods
	
	public void TryMeleeAttack()
	{
		meleeAreaDamage.owner = this; //TODO: change to team alignment
		meleeAreaDamage.damageScale = 1;
		meleeAreaDamage.ApplyDamage();

		animator.SetTrigger("melee");
        Debug.Log("slash!");
	}
	
	public void TryDashSlash()
	{
		
	}
	
	public void TryCastMagic()
	{
		animator.SetTrigger("magic");
		GameObject gobj = GameObject.Instantiate(magicPrefab.gameObject) as GameObject;

		Projectile projectile = gobj.GetComponent<Projectile>();

		projectile.owner = this; //TODO: change to team alignment

		projectile.damageScale = 1;

		projectile.transform.position = lookBody.position;
		projectile.transform.rotation = lookBody.rotation;
	}

	public override void OnDamage (DamagePacket _damagePacket)
	{
		base.OnDamage (_damagePacket);
		animator.SetTrigger("damaged");

		knockBackTime = 0.07f;
		knockBackVelocity = _damagePacket.knockback * knockbackScale;
	}

	public virtual bool GetClosestEnemy(out Killable _target, out float _distance)
	{
		_target = null;
		_distance = 0;

		//if an aggressor is within flee radius, move to a random position
		float minDist = float.MaxValue;
		foreach(CombatEntity aggressor in CombatEntity.activeCombatEntities)
		{
			if(aggressor != this)
			{
				float dist = Vector2.Distance(this.transform.position, aggressor.transform.position);
				if(dist < minDist)
				{
					_distance = dist;
					_target = aggressor;
				}
			}
		}

		return _target != null;
	}
	
	#endregion

	#region monobehaviour methods

	protected void OnEnable()
	{
		activeCombatEntities.Add (this);
	}

	protected void OnDisable()
	{
		activeCombatEntities.Remove (this);
	}

	protected override void Update ()
	{
		if(knockBackTime < 0)
			base.Update ();
		else
		{
			rigidbody2D.velocity = knockBackVelocity;
			knockBackTime -= Time.deltaTime;
		}

		if(meleeAreaDamage)
		{
			meleeAreaDamage.transform.position = lookBody.position;
			meleeAreaDamage.transform.rotation = lookBody.rotation;
		}
	}

	#endregion


}
