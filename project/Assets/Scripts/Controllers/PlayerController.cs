﻿using UnityEngine;
using System.Collections.Generic;

//the player controller controls an eneity via input from the user

public class PlayerController : MonoBehaviour 
{

	#region static variables

	public static List<PlayerController> currentPlayers = new List<PlayerController>();

	#endregion

    #region public variables

	public TeamMember activeEntity = null;
	public TeamMember whiteEntity = null;
	public TeamMember blackEntity = null;
	public List<GameObject> spawnOnTeamChange = new List<GameObject>();

	public bool inputLocked = false;

	public float meleeCooldown = 0.5f;
	public float magicCooldown = 0.3f;
	public float teamChangeCooldown = 0.0f;

	public float magicChargeTime = 1.0f;

    #endregion

    #region private variables

	float meleeCooldownRemaining = 0;
	float magicCooldownRemaining = 0;
	float teamChangeCooldownRemaining = 0;

    #endregion

    #region public methods

    #endregion

    #region private methods

    void TryMove()
    {
        //Handle input 
        Vector2 dir = Vector2.zero;
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir.y = 1;
        }
		else if (Input.GetKey(KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) 
        {
            dir.y = -1;
        }
		if (Input.GetKey(KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
        {
            dir.x = -1;
        }
		else if (Input.GetKey(KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
        {
            dir.x = 1;
        }
        
        activeEntity.TryMove(dir);
    }

    void TryLook()
    {

	        Vector3 botScreenPos = Camera.main.WorldToScreenPoint(activeEntity.transform.position);
	        activeEntity.TryLook(Input.mousePosition - botScreenPos);
	
    }

    void TryAction()
    {
		if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.X)) && meleeCooldownRemaining <= 0)
        {
			meleeCooldownRemaining += meleeCooldown;
			activeEntity.TryMeleeAttack();
        }
		/*else if (Input.GetMouseButtonDown(1) && magicCooldownRemaining <= 0)
		{
			magicCooldownRemaining += magicCooldown;
			activeEntity.TryCastMagic();
		}*/
		else if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z)) && teamChangeCooldownRemaining <= 0 && activeEntity.teamColor != TeamMember.Team.none)
		{
			teamChangeCooldownRemaining += teamChangeCooldown;
			//TODO: better support for multiple teams
			if(activeEntity.teamColor == TeamMember.Team.red)
			{
				OnTeamChange(TeamMember.Team.blue);
			}
			else if(activeEntity.teamColor == TeamMember.Team.blue)
			{
				OnTeamChange(TeamMember.Team.red);
			}
		}
    }

	void OnTeamChange(TeamMember.Team _team)
	{
		TeamMember previousEntity = activeEntity;

		if(_team == TeamMember.Team.red)
		{
			blackEntity.gameObject.SetActive(false);
			activeEntity = whiteEntity;
		}
		else if(_team == TeamMember.Team.blue)
		{
			whiteEntity.gameObject.SetActive(false);
			activeEntity = blackEntity;
		}

		activeEntity.gameObject.SetActive(true);
		activeEntity.transform.position = previousEntity.transform.position;

		foreach(GameObject gobj in spawnOnTeamChange)
		{
			GameObject spawned = Instantiate(gobj) as GameObject;
			spawned.transform.position = activeEntity.transform.position;
		}

		GameManager.Instance.dirtyScore = true;
	}

    #endregion

    #region monobehaviour methods

	// Use this for initialization
	void Awake () 
    {
		OnTeamChange(activeEntity.teamColor);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (activeEntity)
        {
			if(!inputLocked)
			{
                TryMove();
                TryLook();
                TryAction();
			}
        }
		else //cleanup
		{
			Destroy(gameObject);
		}

		meleeCooldownRemaining = Mathf.Max(0, meleeCooldownRemaining - Time.deltaTime);
		magicCooldownRemaining = Mathf.Max(0, magicCooldownRemaining - Time.deltaTime);
		teamChangeCooldownRemaining = Mathf.Max(0, teamChangeCooldownRemaining - Time.deltaTime);

		//FIXME: DEBUG RESET
		if(Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void OnEnable()
	{
		currentPlayers.Add(this);
	}

    void OnDisable()
    {
		currentPlayers.Remove(this);

		if(activeEntity)
        	activeEntity.TryMove(Vector2.zero);
    }

    #endregion
}
