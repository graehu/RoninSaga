using UnityEngine;
using System.Collections;

public class MagicAIController : MonoBehaviour
{
	#region public types

	public enum State
	{
		Idle,
		Attacking,
		Moving
	}

	#endregion

	#region public variables

	public CombatEntity entity;
	public Vector3 targetPosition;
	public float targetThreshold = 0.15f;

	public float stateDelay = 0.5f;
	public float fleeRadius = 0.5f;

	public int consecutiveAttacks = 2;
	public float attackDelay = 0.5f;
	public float attackInterval = 0.5f;

	public float agressiveness = 0.6f;
	public float curiousness = 0.4f;

	#endregion

	#region private variables

	State currentState = State.Idle;
	float stateTick = 0;

	int attackCount = 0;

	Killable targetToAttack = null;

	#endregion

	#region private methods

	void SetState(State _newState)
	{
		currentState = _newState;
		stateTick = 0;
	}

	#endregion

	#region monobehaviour methods
	
	// Use this for initialization
	void Start () 
	{
		//set target as current position to begin
		targetPosition = transform.position; 
	}
	
	// Update is called once per frame
	void Update () 
	{
		stateTick += Time.deltaTime;

		if(stateTick < stateDelay)
			return;

		switch(currentState)
		{
		case State.Idle:
		{
			//if a player is within flee radius, move to a random position
			foreach(PlayerController player in PlayerController.currentPlayers)
			{
				if(Vector2.Distance(this.transform.position, player.transform.position) < fleeRadius)
				{
					targetPosition = RoomHelper.RandomPosition();
					SetState(State.Moving);
					return;
				}
			}

			if(Random.value < agressiveness)
			{
				SetState(State.Attacking);
			}
			else
			{
				targetPosition = RoomHelper.RandomPosition();
				SetState(State.Moving);
			}

			entity.TryMove(Vector2.zero);
			entity.TryLook(new Vector2(Random.Range(-1,1),Random.Range(-1,1)));

			break;
		}
		case State.Attacking:
		{
			//try to find target
			if(targetToAttack == null)
			{
				float minDist = float.MaxValue;
				foreach(PlayerController player in PlayerController.currentPlayers)
				{
					float dist = Vector2.Distance(this.transform.position, player.transform.position);
					if(dist < minDist)
					{
						minDist = dist;
						targetToAttack = player.entity;
					}
				}
				attackCount = 0;
			}
			else
			{
				//check if we've finished attacking
				if(attackCount >= consecutiveAttacks)
				{
					targetToAttack = null;
					targetPosition = RoomHelper.RandomPosition();
					SetState(State.Moving);
				}
				//try to attack
				else if(stateTick > stateDelay + attackDelay + (attackCount*attackInterval))
				{
					attackCount++;
					entity.TryCastMagic();
				}

				entity.TryLook(targetToAttack.transform.position - this.transform.position);

			}

			entity.TryMove(Vector2.zero);

			break;
		}
		case State.Moving:
		{
			Vector2 distance = targetPosition - entity.transform.position;
			if(distance.magnitude < targetThreshold)
			{
				SetState(State.Idle);
			}
			else
			{
				entity.TryMove(distance);
				entity.TryLook(distance);
			}
			break;
		}
		}
	}

	void OnCollisionStay2D(Collision2D _collision)
	{
		if(_collision.collider.tag == "Player")
		{
			targetPosition = this.transform.position + (this.transform.position - _collision.collider.transform.position).normalized * fleeRadius;
			targetPosition = RoomHelper.BoundToRoom(targetPosition);
			SetState(State.Moving);
		}
	}

	#endregion
}
